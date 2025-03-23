using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebApi
{
    public class PolicyProcessingOrchestrator
    {
        private readonly IPolicySummarizer _policySummarizer;
        private readonly ILogger<PolicyProcessingOrchestrator> _logger;

        public PolicyProcessingOrchestrator(
            IPolicySummarizer policySummarizer,
            ILogger<PolicyProcessingOrchestrator> logger)
        {
            _policySummarizer = policySummarizer;
            _logger = logger;
        }

        public async Task<PolicyResult> ProcessPolicyFile(string filePath)
        {
            try
            {
                _logger.LogInformation($"Processing policy file: {filePath}");

                if (!File.Exists(filePath))
                {
                    _logger.LogError($"File not found: {filePath}");
                    throw new FileNotFoundException($"Policy file not found", filePath);
                }

                string summary = await _policySummarizer.SummarizePolicy(filePath);

                return new PolicyResult
                {
                    FilePath = filePath,
                    FileName = Path.GetFileName(filePath),
                    Summary = summary,
                    ProcessedDate = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing policy file: {filePath}");
                throw;
            }
        }
    }

    public class PolicyResult
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Summary { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}