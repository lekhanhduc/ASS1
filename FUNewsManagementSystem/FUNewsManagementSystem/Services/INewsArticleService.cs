using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;

namespace FUNewsManagementSystem.Services
{
    public interface INewsArticleService
    {
        Task<NewsArticleCreationResponse> CreateNewsArticleAsync(NewsArticleCreationRequest request);
        Task<PageResponse<NewsArticleDetailResponse>> GetAllNewsArticlesAsync(int page, int size);
        Task<PageResponse<NewsArticleDetailResponse>> GetNewsArticlesByUserLogin(int page, int size);
        Task<NewsArticleDetailResponse?> GetNewsArticleByIdAsync(int id);
        Task DeleteNewsArticleAsync(int id);
    }
}
