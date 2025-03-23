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

        public PdfParser(string filePath, string documentType )
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
    string pdfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Insurance.pdf");

    if (!File.Exists(pdfPath))
    {
        throw new FileNotFoundException($"PDF file not found at path: {pdfPath}");
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
                                  !words[k].Text.EndsWith(":") && !words[k].Text.EndsWith("-") && !words[k].Text.EndsWith("="))
                            {
                                valueBuilder.Append(words[k].Text).Append(" ");
                                valueWordCount++;
                                k++;
                            }

                            string value = valueBuilder.ToString().Trim();

                            if (!string.IsNullOrWhiteSpace(label) && !string.IsNullOrWhiteSpace(value))
                            {
                                label = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(label.ToLower());

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

        private void IdentifyCommonInsuranceFields(string pageText, Dictionary<string, string> keyValuePairs)
        {
            var patterns = new Dictionary<string, string>
            {
                //Update this with all the policies we need to update;
                { "Policy Number", @"Policy\s*(Number|No|#)[\s:\-]*([A-Z0-9\-]{5,20})" },
                { "Policy Period", @"Policy\s*Period[\s:\-]*(.*?)(?=\n)" },
                { "Insured Name", @"(Named\s*Insured|Insured)[\s:\-]*(.*?)(?=\n)" },
                { "Premium Amount", @"(Total\s*Premium|Premium)[\s:\-]*\$?([\d,]+\.?\d*)" },
                { "Coverage Limit", @"(Coverage\s*Limit|Limit of Liability)[\s:\-]*\$?([\d,]+\.?\d*)" },
                { "Deductible", @"Deductible[\s:\-]*\$?([\d,]+\.?\d*)" }
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(pageText, pattern.Value, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 2)
                {
                    string value = match.Groups[2].Value.Trim();
                    if (!string.IsNullOrWhiteSpace(value) && !keyValuePairs.ContainsKey(pattern.Key))
                    {
                        keyValuePairs.Add(pattern.Key, value);
                    }
                }
            }
        }

        public Dictionary<string, dynamic> ExtractPolicyInfo()
        {
            var policyInfo = new Dictionary<string, dynamic>();
            string fullText = ExtractTextFromPdf();

            return policyInfo;
        }
    }
}