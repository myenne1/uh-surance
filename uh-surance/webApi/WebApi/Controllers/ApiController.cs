using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

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
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadPolicyReciept(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "Only PDF files are accepted" });
            }

            try
            {
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
        
        [HttpGet("insurance-fields/{id}")]
        public IActionResult GetInsuranceFields(Guid id)
        {
            var policy = ProcessedPolicies.FirstOrDefault(p => p.Id == id);
            if (policy == null)
            {
                return NotFound(new { error = "Policy not found" });
            }

            try
            {
                var pdfParser = new PdfParser(policy.FilePath, "Reciept");
                var fields = pdfParser.ExtractKeyValuePairs(policy.FilePath);
                return Ok(fields);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting insurance fields: {ex.Message}");
                return StatusCode(500, new { error = "Error extracting insurance fields" });
            }
        }
    }

    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _uploadDirectory;

        public ImageController(IConfiguration configuration)
        {
            _configuration = configuration;
            _uploadDirectory = _configuration["FileStorage:ImageUploadDirectory"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Images");

            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        [HttpPost("upload")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { error = "Only image files are accepted" });
            }

            try
            {
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(_uploadDirectory, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                string imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/images/{uniqueFileName}";

                return Ok(new
                {
                    fileName = uniqueFileName,
                    originalName = file.FileName,
                    filePath = filePath,
                    url = imageUrl,
                    uploadDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return StatusCode(500, new { error = "Error uploading the image file" });
            }
        }

        [HttpGet]
        public IActionResult GetImages()
        {
            try
            {
                var files = Directory.GetFiles(_uploadDirectory)
                    .Select(f => new {
                        fileName = Path.GetFileName(f),
                        filePath = f,
                        url = $"{Request.Scheme}://{Request.Host}/uploads/images/{Path.GetFileName(f)}",
                        uploadDate = System.IO.File.GetCreationTime(f)
                    });

                return Ok(files);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving images: {ex.Message}");
                return StatusCode(500, new { error = "Error retrieving images" });
            }
        }
    }

    [ApiController]
[Route("api/insurance")]
public class InsuranceFieldController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly string _uploadDirectory;

    public InsuranceFieldController(IConfiguration configuration)
    {
        _configuration = configuration;
        _uploadDirectory = _configuration["FileStorage:InsuranceFieldsDirectory"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "InsuranceFields");

        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    [HttpPost("{fieldType}/upload")]
    [RequestSizeLimit(20 * 1024 * 1024)] 
    public async Task<IActionResult> UploadFieldImage(string fieldType, IFormFile file, CancellationToken cancellationToken)
    {
        if (!IsValidFieldType(fieldType))
        {
            return BadRequest(new { error = $"Field type '{fieldType}' is not supported. Supported types: Electronics, Jewelry" });
        }

        if (file == null || file.Length <= 0)
        {
            return BadRequest(new { error = "No file uploaded" });
        }

        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
        string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(new { error = "Only image files are accepted" });
        }

        try
        {
            string fieldDirectory = Path.Combine(_uploadDirectory, fieldType);
            if (!Directory.Exists(fieldDirectory))
            {
                Directory.CreateDirectory(fieldDirectory);
            }

            string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(fieldDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            string imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/InsuranceFields/{fieldType}/{uniqueFileName}";

            return Ok(new
            {
                fieldType = fieldType,
                fileName = uniqueFileName,
                originalName = file.FileName,
                filePath = filePath,
                url = imageUrl,
                uploadDate = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading {fieldType} image: {ex.Message}");
            return StatusCode(500, new { error = $"Error uploading the {fieldType} image file" });
        }
    }

    [HttpGet("{fieldType}")]
    public IActionResult GetFieldImages(string fieldType)
    {
        // Validate fieldType is one we support
        if (!IsValidFieldType(fieldType))
        {
            return BadRequest(new { error = $"Field type '{fieldType}' is not supported. Supported types: Electronics, Jewelry" });
        }

        try
        {
            string fieldDirectory = Path.Combine(_uploadDirectory, fieldType);
            
            if (!Directory.Exists(fieldDirectory))
            {
                // Return empty list if directory doesn't exist yet
                return Ok(new List<object>());
            }

            var files = Directory.GetFiles(fieldDirectory)
                .Select(f => new {
                    fieldType = fieldType,
                    fileName = Path.GetFileName(f),
                    filePath = f,
                    url = $"{Request.Scheme}://{Request.Host}/uploads/InsuranceFields/{fieldType}/{Path.GetFileName(f)}",
                    uploadDate = System.IO.File.GetCreationTime(f)
                });

            return Ok(files);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving {fieldType} images: {ex.Message}");
            return StatusCode(500, new { error = $"Error retrieving {fieldType} images" });
        }
    }

    private bool IsValidFieldType(string fieldType)
    {
        string[] supportedTypes = { "Electronics", "Jewelry" };
        return supportedTypes.Contains(fieldType, StringComparer.OrdinalIgnoreCase);
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