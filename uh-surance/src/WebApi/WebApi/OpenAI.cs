using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

namespace WebApi
{
    public sealed class PolicySummarizerService : IPolicySummarizer
    {
        private readonly string _apiKey;

        // Proper detailed prompt for insurance policy summarization
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
            _apiKey = configuration["OpenAI:ApiKey"]
                ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new ArgumentNullException("OpenAI API key is missing");
        }

        public async Task<string> SummarizePolicy(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"No policy found at: {filePath}");

            // Read file content as text (assuming it's already extracted from PDF)
            string policyText = await File.ReadAllTextAsync(filePath);

            // If this is binary (like PDF), you'll need to extract text first

            //Extract Text here
            
            var client = new ChatClient(
                model: "gpt-4o",
                apiKey: _apiKey
            );

            // Configure chat completion options
            var completionOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 1000, // Larger for comprehensive summary
                Temperature = 0.7f,         // Good balance for summarization
                FrequencyPenalty = 0.0f,
                PresencePenalty = 0.5f,     // Slight penalty to avoid repetition
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