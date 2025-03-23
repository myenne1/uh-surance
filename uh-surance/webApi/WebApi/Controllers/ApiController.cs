using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/policies")]
    public class PolicyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPolicySummarizer _policySummarizer;
        private readonly string _uploadDirectory;
        private static readonly List<PolicyRecord> ProcessedPolicies = new List<PolicyRecord>();

        public PolicyController(IConfiguration configuration, IPolicySummarizer policySummarizer)
        {
            _configuration = configuration;
            _policySummarizer = policySummarizer;

            _uploadDirectory = _configuration["FileStorage:UploadDirectory"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Policies");

            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
        public async Task<IActionResult> UploadPolicy(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            // Validate file is PDF
            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "Only PDF files are accepted" });
            }

            try
            {
                // Create unique filename
                string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                string filePath = Path.Combine(_uploadDirectory, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                string summary = await _policySummarizer.SummarizePolicy(filePath);

                var policyRecord = new PolicyRecord
                {
                    Id = Guid.NewGuid(),
                    FileName = uniqueFileName,
                    FilePath = filePath,
                    Summary = summary,
                    UploadDate = DateTime.Now
                };

                ProcessedPolicies.Add(policyRecord);

                return Ok(policyRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing policy: {ex.Message}");
                return StatusCode(500, new { error = "Error processing the policy file" });
            }
        }

        [HttpGet]
        public IActionResult GetPolicies()
        {
            return Ok(ProcessedPolicies);
        }
    }

    public class PolicyRecord
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Summary { get; set; }
        public DateTime UploadDate { get; set; }
    }
}