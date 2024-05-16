using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using AspNetCoreRestApi.Helpers;
using AspNetCoreRestApi.Models;

namespace AspNetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly FileUploader _uploader;
        private readonly IConfiguration _configuration;

        public UploadsController(IConfiguration configuration)
        {
            _configuration = configuration;
            var uploadsDirectory = _configuration["UploadsDirectory"];
            // Initialize the uploader with destination path, max size, and allowed extensions
            _uploader = new FileUploader(uploadsDirectory, FileExtensions.Images, 10 * CapacityUnit.MEGA_BYTE);
            
        }

        
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                // Upload the file using the Uploader class
                var uploadInfo = await _uploader.UploadSingleFile(HttpContext.Request, file.Name);

                // Return the uploaded file information
                return Ok(uploadInfo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
