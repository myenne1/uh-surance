using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApi
{
    public class PdfParser
    {
        private readonly string[] punctuation = new[] { ".", "," };
        private readonly string[] cannotStartWord = new[] { ")", "]", "!", "?", ";", ":" };
        private readonly string[] cannotEndWord = new[] { "(", "[" };

        // Remove the reference to a non-existent Orchestrator class
        private readonly string _filePath;

        public PdfParser(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
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

                    // Add a newline at the end of each page
                    extractedText.AppendLine();
                }
            }

            return extractedText.ToString();
        }

        // You can add more specialized parsing methods here
        public Dictionary<string, dynamic> ExtractPolicyInfo()
        {
            var policyInfo = new Dictionary<string, dynamic>();
            string fullText = ExtractTextFromPdf();

            return policyInfo;
        }
    }
}