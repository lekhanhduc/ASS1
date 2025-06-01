using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse<CategoryCreationResponse>> CreateCategory([FromBody] CategoryCreationRequest request)
        {
            var ressult = await categoryService.CreateCategory(request);
            return new ApiResponse<CategoryCreationResponse>
            {
                code = 201,
                message = "Category created successfully",
                result = ressult
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<CategoryDetailResponse>>> GetAllCategories()
        {
            var result = await categoryService.GetAll();
            return new ApiResponse<List<CategoryDetailResponse>>
            {
                code = 200,
                message = "Categories retrieved successfully",
                result = result
            };

        }
    }
}
