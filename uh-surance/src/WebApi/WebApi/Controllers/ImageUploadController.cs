// authenticate and authorize
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

// seal the class and only derive from what you need
public sealed class ImageUploadController : Controller
{
    private string? actualpath;

    // use logging (i recommend Serilog)
    // inject ILogger<ImageUploadController> for using the native ASP.NET logging infrastructure

    // use Dependency Injection
    public ImageUploadController(IConfiguration configuration)
    {
        // assert your injects are properly done
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private IConfiguration Configuration { get; }

    // define routes and HTTP methods
    [HttpPost("api/files")]
    // prevent cross site scripting attacks
    [ValidateAntiForgeryToken]
    // always return IActionResult; provide cancellation token, such that the client can cancel the request
    public async Task<IActionResult> Post([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        // validate the model
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // additional input validation
        if (file == null || file.Length <= 0) return BadRequest();

        // TODO: put the business logic (everything within this try block) into a dedicated class that gets injected
        // this saves you from writing complex integration tests instead of simple unit tests
        try
        {

            // don't hardcode paths, but make them configurable
            // otherwise you shoot yourself into your foot by not being able to unit test your code
            var actualPath = Configuration["actualPath"];
            if (!Directory.Exists(actualpath))
            {
                // not sure if async is useful here; it's an I/O operation, but usually also fast, so not sure if creating a task makes sense here
                await Task.Run(() => Directory.CreateDirectory(actualpath));
            }

            // don't concatenate path-strings; always use Path.Combine
            // also the file name is not unique, use a hash of the file data or an unique ID (and check if the file already exists)
            var filePath = Path.Combine(actualpath, file.FileName);

            using (var filestream = System.IO.File.Create(filePath, 1024, FileOptions.SequentialScan)) // specify file options when creating a file for better performance
            {
                // use async/await such that the server can handle another request until the file is written
                await file.CopyToAsync(filestream, cancellationToken);
            } // close stream as early as possible; disposing also flushes
        }
        // use custom exception wrapper to prevent double logging of exception
        // (not used here; just for illustration purpose)
        catch (Exception ex)
        {
            // log error and return HTTP Code 500 on Exceptions
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // return the URL where the file can be retrived
        var fileUrlBuilder = new UriBuilder(Request.GetEncodedUrl());
        fileUrlBuilder.Query = null;
        fileUrlBuilder.Fragment = null;
        fileUrlBuilder.Path = Path.Combine(fileUrlBuilder.Path, file.FileName);
        return Created(fileUrlBuilder.Uri, new AcceptedResult()); // HTTP Code 201 with URL to file
    }
}