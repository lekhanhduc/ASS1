using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Controllers
{
    [Route("odata/Categories")]
    public class SearchCategoryController : ODataController
    {

        private readonly AppDbContext dbContext;
        public SearchCategoryController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        public IQueryable<Category> Get()
        {
            return dbContext.Categories
                .Where(c => c.IsActive);
        }

        [HttpGet("{categoryId}/NewsArticles")]
        [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        public IActionResult GetNewsArticlesByCategoryId(int categoryId)
        {
            // Kiểm tra xem CategoryId có tồn tại và hợp lệ không
            var categoryExists = dbContext.Categories
                .Any(c => c.CategoryId == categoryId && c.IsActive);

            if (!categoryExists)
            {
                return NotFound($"Không tìm thấy Category với ID {categoryId} hoặc Category không hoạt động.");
            }

            // Lấy danh sách NewsArticle thuộc CategoryId
            var newsArticles = dbContext.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.SystemAccount)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.CategoryId == categoryId && n.NewsStatus == "ACTIVE");

            return Ok(newsArticles);
        }

    }
}
