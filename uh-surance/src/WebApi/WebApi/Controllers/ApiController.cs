using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly string _policyFolder;
    public FilesController(IWebHostEnvironment environment)
    {
        _policyFolder = Path.Combine(environment.ContentRootPath, "Uploads");

        // Ensure the uploads directory exists
        if (!Directory.Exists(_policyFolder))
        {
            Directory.CreateDirectory(_policyFolder);
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No file was uploaded" });
        }

        try
        {
            // Generate a unique filename to prevent conflicts
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_policyFolder, fileName);

            // Save the file
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return success response with file information
            return Ok(new
            {
                message = "File uploaded successfully",
                fileName,
                size = file.Length,
                contentType = file.ContentType
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
        }


    }
}