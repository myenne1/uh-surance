using System;
using System.IO;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public sealed class PolicySummarizerService : IPolicySummarizer
    {
        private readonly string _apiKey;

        // Prompt for insurance policy summarization
        private const string SUMMARIZE_PROMPT = @"You are an insurance expert. Analyze the following insurance policy
and provide a concise summary that includes:
1. Type of insurance (auto, home, health, etc.)
2. Coverage limits
3. Important exclusions
4. Deductible amounts
5. Key dates (effective period)
6. Monthly/annual premium
7. Special conditions

Summarize in plain language that a non-expert would understand:";

        public PolicySummarizerService(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAIKey:ApiKey"]
                ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new ArgumentNullException("OpenAI API key is missing");
        }

        public async Task<string> SummarizePolicy(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"No policy found at: {filePath}");

            // Use the PdfParser to extract text
            var pdfParser = new PdfParser(filePath);
            string policyText = pdfParser.ExtractTextFromPdf();

            if (string.IsNullOrWhiteSpace(policyText))
            {
                throw new InvalidOperationException("Could not extract text from the PDF file");
            }

            var client = new ChatClient(
                model: "gpt-4o",
                apiKey: _apiKey
            );

            // Configure chat completion options
            var completionOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 1000,
                Temperature = 0.7f,
                FrequencyPenalty = 0.0f,
                PresencePenalty = 0.5f,
            };

            // Build messages with system prompt and user content
            string promptMessage = SUMMARIZE_PROMPT + policyText;
            try
            {
                // Execute the completion request
                var response = await client.CompleteChatAsync(promptMessage);

                // Return the summary
                return response.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error calling OpenAI API: {ex.Message}");
                throw new Exception("Failed to generate policy summary", ex);
            }
        }
    }
}