using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/v1/file")]
    [ApiController]
    public class FileController : ControllerBase
    {

        private readonly CloudinaryService _cloudinaryService;
        private readonly ILogger<FileController> _logger;

        public FileController(CloudinaryService cloudinaryService, ILogger<FileController> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<ApiResponse<string>> UploadImage(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var imageUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName);
                return new ApiResponse<string>
                {
                    code = 200,
                    message = "Image uploaded successfully.",
                    result = imageUrl
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return new ApiResponse<string>
                {
                    code = 400,
                    message = "Image uploaded failed",
                };
            }
        }

    }
}
