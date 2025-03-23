using PdfSharp.Snippets.Font;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;


namespace WebApi
{
    /*
     * Things to parse:
     * Things are covered, limit of covered items
     * What is cove
     */
    public class PdfParser
    {
        string[] punctuation = new[] { ".", "," };
        string[] cannotStartWord = new[] { ")", "]", "!", "?", ";", ":" };
        string[] cannotEndWord = new[] { "(", "[" };
        private FileStream pdfFileStream;
        private StreamWriter outPutStreamWriter;

        private Dictionary<string, dynamic> receiptValues = new Dictionary<string, dynamic>();

        public void parseTitleData()
        {
            int rotatingCounter = 0;

            using (var document = PdfDocument.Open(Orchestrator.filePath))
            {
                for (var i = 0; i < document.NumberOfPages; i++)
                {
                    var page = document.GetPage(i + 1);
                    var nnOptions = new NearestNeighbourWordExtractor.NearestNeighbourWordExtractorOptions()
                    {
                        // Ignore the letters that are space or belong to 'punctuation' array
                        // These letters will be put in a single word
                        FilterPivot = letter => !string.IsNullOrWhiteSpace(letter.Value) &&
                                                !punctuation.Contains(letter.Value),

                        Filter = (pivot, candidate) =>
                        {
                            if (string.IsNullOrWhiteSpace(candidate.Value) ||
                                cannotEndWord.Contains(candidate.Value))
                            {
                                // start new word if the candidate neighbour is 
                                // a space or belongs to 'cannotEndWord' array
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
                        Console.WriteLine(word);
                    }
                }
            }
        }
    }
}
