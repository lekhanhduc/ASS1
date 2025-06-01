using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/v1/newsaricles")]
    [ApiController]
    public class NewsArticleController : ControllerBase
    {

        private readonly INewsArticleService newsArticleService;
        public NewsArticleController(INewsArticleService newsArticleService)
        {
            this.newsArticleService = newsArticleService;
        }


        [HttpPost]
        [Authorize]
        public async Task<ApiResponse<NewsArticleCreationResponse>> CreateNewsArticle([FromForm] NewsArticleCreationRequest request)
        {
           var result = await newsArticleService.CreateNewsArticleAsync(request);
            return new ApiResponse<NewsArticleCreationResponse>
            {
                code = 200,
                message = "News article created successfully",
                result = result
            };
        }

        [HttpGet]
        public async Task<ApiResponse<PageResponse<NewsArticleDetailResponse>>> GetAllNewsArticles(
            [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await newsArticleService.GetAllNewsArticlesAsync(page, size);
            return new ApiResponse<PageResponse<NewsArticleDetailResponse>>
            {
                code = 200,
                message = "News articles retrieved successfully",
                result = response
            };
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<ApiResponse<PageResponse<NewsArticleDetailResponse>>> GetNewsArticlesByUserLogin(
            [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await newsArticleService.GetNewsArticlesByUserLogin(page, size);
            return new ApiResponse<PageResponse<NewsArticleDetailResponse>>
            {
                code = 200,
                message = "User's news articles retrieved successfully",
                result = response
            };
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ApiResponse<NewsArticleDetailResponse?>> GetNewsArticleByIdAsync(int id)
        {
            var response = await newsArticleService.GetNewsArticleByIdAsync(id);
            return new ApiResponse<NewsArticleDetailResponse?>
            {
                code = 200,
                message = "News article retrieved successfully",
                result = response
            };
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "STAFF")]
        public async Task<ApiResponse<string>> DeleteNewsArticleAsync(int id)
        {
            await newsArticleService.DeleteNewsArticleAsync(id);
            return new ApiResponse<string>
            {
                code = 200,
                message = "News article deleted successfully"
            };
        }

    }
}
