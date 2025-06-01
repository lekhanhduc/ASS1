using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Middlewares;
using FUNewsManagementSystem.Repositories;

namespace FUNewsManagementSystem.Services.Impl
{
    public class CategoryService : ICategoryService
    {

        private readonly CategoryRepository categoryRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SystemAccountRepository systemAccountRepository;

        public CategoryService(CategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor,
            SystemAccountRepository systemAccountRepository)
        {
            this.categoryRepository = categoryRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.systemAccountRepository = systemAccountRepository;
        }


        public async Task<CategoryCreationResponse> CreateCategory(CategoryCreationRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var accountId = int.Parse(accountIdClaim);
            var user = await systemAccountRepository.FindById(accountId);

            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }

            var category = await categoryRepository.FindByCategoryName(request.CategoryName);
            if (category != null)
            {
                throw new AppException(ErrorCode.CATEGORY_EXISTED);
            }

            var newCategory = new Models.Category
            {
                CategoryName = request.CategoryName,
                Description = request.Description,
                IsActive = true
            };

            var createdCategory = await categoryRepository.CreateCategoryAsync(newCategory);

            return new CategoryCreationResponse(
                createdCategory.CategoryId,
                createdCategory.CategoryName,
                createdCategory.Description,
                createdCategory.IsActive
            );
        }

        public async Task<List<CategoryDetailResponse>> GetAll()
        {
            var categories = await categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryDetailResponse(
                c.CategoryId,
                c.CategoryName,
                c.Description,
                c.IsActive,
                c.ParentCategoryID != null ? c.ParentCategoryID.Value : 0
            )).ToList();
        }

    }
}
