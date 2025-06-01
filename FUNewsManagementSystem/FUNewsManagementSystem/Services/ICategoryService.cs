using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;

namespace FUNewsManagementSystem.Services
{
    public interface ICategoryService
    {
        Task<CategoryCreationResponse> CreateCategory(CategoryCreationRequest request);
        Task<List<CategoryDetailResponse>> GetAll();
    }
}
