using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebApi
{
    public class PdfParser
    {
        private readonly string[] punctuation = new[] { ".", "," };
        private readonly string[] cannotStartWord = new[] { ")", "]", "!", "?", ";", ":" };
        private readonly string[] cannotEndWord = new[] { "(", "[" };

        private string _filePath = "";
        private string InsurancePath = "";

        public PdfParser(string filePath, string documentType)
        {
            if (documentType == "Insurance")
            {
                InsurancePath = filePath;
                return;
            }
            else
            {
                _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            }
        }

       public string ExtractRawText()
{
    // Option 1: Look for the file in multiple locations
    string[] possiblePaths = new[] {
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Insurance.pdf"),
        Path.Combine(Directory.GetCurrentDirectory(), "Insurance.pdf"),
        InsurancePath // Use the path passed to the constructor if available
    };
    
    string pdfPath = possiblePaths.FirstOrDefault(File.Exists);
    
    if (pdfPath == null)
    {
        throw new FileNotFoundException($"PDF file not found at any of the tried paths: {string.Join(", ", possiblePaths)}");
    }

    var extractedText = new System.Text.StringBuilder();

            using (var document = PdfDocument.Open(pdfPath))
            {
                for (var i = 0; i < document.NumberOfPages; i++)
                {
                    var page = document.GetPage(i + 1);

                    var text = page.Text;
                    extractedText.AppendLine(text);

                    extractedText.AppendLine($"--- Page {i + 1} ---");
                }
            }

            return extractedText.ToString();
        }

        public string ExtractTextFromPdf()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"PDF file not found at path: {_filePath}");
            }

            var extractedText = new System.Text.StringBuilder();

            using (var document = PdfDocument.Open(_filePath))
            {
                for (var i = 0; i < document.NumberOfPages; i++)
                {
                    var page = document.GetPage(i + 1);
                    var nnOptions = new NearestNeighbourWordExtractor.NearestNeighbourWordExtractorOptions()
                    {
                        FilterPivot = letter => !string.IsNullOrWhiteSpace(letter.Value) &&
                                                !punctuation.Contains(letter.Value),

                        Filter = (pivot, candidate) =>
                        {
                            if (string.IsNullOrWhiteSpace(candidate.Value) ||
                                cannotEndWord.Contains(candidate.Value))
                            {
                                return false;
                            }
                            else if (cannotStartWord.Contains(pivot.Value))
                            {
                                return false;
                            }

                            return true;
                        }
                    };

                    var nnExtract = new NearestNeighbourWordExtractor(nnOptions).GetWords(page.Letters);

                    foreach (var word in nnExtract)
                    {
                        extractedText.Append(word.Text).Append(" ");
                    }

                    extractedText.AppendLine();
                }
            }

            return extractedText.ToString();
        }

        public Dictionary<string, string> ExtractKeyValuePairs(string RecieptFilePath)
        {
            if (!File.Exists(RecieptFilePath))
            {
                throw new FileNotFoundException($"PDF file not found at path: {RecieptFilePath}");
            }

            var keyValuePairs = new Dictionary<string, string>();

            using (var document = PdfDocument.Open(RecieptFilePath))
            {
                for (var i = 0; i < document.NumberOfPages; i++)
                {
                    var page = document.GetPage(i + 1);

                    var words = NearestNeighbourWordExtractor.Instance.GetWords(page.Letters).ToList();

                    for (int j = 0; j < words.Count - 1; j++)
                    {
                        var currentWord = words[j].Text.Trim();

                        if (currentWord.EndsWith(":") || currentWord.EndsWith("-") || currentWord.EndsWith("="))
                        {
                            string label = currentWord.TrimEnd(':', '-', '=', ' ');

                            // Build value from next few words (up to 5 words or until another potential label)
                            var valueBuilder = new System.Text.StringBuilder();
                            int valueWordCount = 0;
                            int k = j + 1;

                            while (k < words.Count && valueWordCount < 5 &&
                                   !words[k].Text.EndsWith(":") && !words[k].Text.EndsWith("-") &&
                                   !words[k].Text.EndsWith("="))
                            {
                                valueBuilder.Append(words[k].Text).Append(" ");
                                valueWordCount++;
                                k++;
                            }

                            string value = valueBuilder.ToString().Trim();

                            if (!string.IsNullOrWhiteSpace(label) && !string.IsNullOrWhiteSpace(value))
                            {
                                label = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                                    label.ToLower());

                                if (!keyValuePairs.ContainsKey(label))
                                {
                                    keyValuePairs.Add(label, value);
                                }
                            }
                        }
                    }

                    IdentifyCommonInsuranceFields(page.Text, keyValuePairs);
                }
            }

            return keyValuePairs;
        }

        // Updated regex patterns to include policy coverage and accepted coverage fields
        private void IdentifyCommonInsuranceFields(string pageText, Dictionary<string, string> keyValuePairs)
        {
            var patterns = new Dictionary<string, string>
            {
                // Existing patterns
                { "Policy Type", @"Policy\s*(Number|No|#)[\s:\-]*([A-Z0-9\-]{5,20})" },
                { "Policy Period", @"Policy\s*Period[\s:\-]*(.*?)(?=\n)" },
                { "Insured Name", @"(Named\s*Insured|Insured|Applicant Name)[\s:\-]*(.*?)(?=\n)" },
                { "Premium Amount", @"(Total\s*Premium|Premium)[\s:\-]*\$?([\d,]+\.?\d*)" },
                { "Coverage Limit", @"(Coverage\s*Limit|Limit of Liability)[\s:\-]*\$?([\d,]+\.?\d*)" },
                { "Deductible", @"(Policy\s*Deductible|Deductible)[\s:\-]*\$?([\d,]+\.?\d*)" },

                // New patterns for policy coverage
                { "Personal Property", @"Personal\s*Property\s*\(Coverage\s*B\)[\s:\-]*\$?([\d,]+)" },
                { "Personal Liability", @"Personal\s*Liability\s*\(Coverage\s*L\)[\s:\-]*\$?([\d,]+)" },
                { "Medical Payments", @"Medical\s*Payments\s*\(Coverage\s*M\)[\s:\-]*\$?([\d,]+)" },
                { "Credit Card", @"Credit\s*Card\s*\/\s*Bank\s*Card[\s:\-]*\$?([\d,]+)" },
                { "Damage to Property", @"Damage\s*to\s*Property\s*of\s*Others[\s:\-]*\$?([\d,]+)" },
                { "Loss of Use", @"Loss\s*of\s*Use[\s:\-]*\$?([\d,]+)" },

                // Loss Settlement Option
                {
                    "Loss Settlement Option", @"Loss\s*Settlement\s*Option\s*-\s*Personal\s*Property[\s:\-]*(.*?)(?=\n)"
                },

                // Accepted Options patterns
                { "Jewelry and Furs", @"Jewelry\s*and\s*Furs[\s:\-]*\$?([\d,]+)\s*included" },
                {
                    "Silver Goldware Theft", @"Silver\/Goldware\s*Theft\s*-\s*Option\s*SG[\s:\-]*\$?([\d,]+)\s*included"
                },
                { "Business Property", @"Business\s*Property\s*-\s*Option\s*BP[\s:\-]*\$?([\d,]+)\s*included" },
                { "Firearms", @"Firearms\s*-\s*Option\s*FA[\s:\-]*\$?([\d,]+)\s*included" }
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(pageText, pattern.Value, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    string value = match.Groups[1].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(value) && !keyValuePairs.ContainsKey(pattern.Key))
                    {
                        keyValuePairs.Add(pattern.Key, value);
                    }
                }
            }
        }

        public Dictionary<string, string> ExtractPolicyInfo()
        {
            var policyInfo = new Dictionary<string, string>();
            string fullText = ExtractTextFromPdf();
            IdentifyCommonInsuranceFields(fullText, policyInfo);
            return policyInfo;
        }
    }
}