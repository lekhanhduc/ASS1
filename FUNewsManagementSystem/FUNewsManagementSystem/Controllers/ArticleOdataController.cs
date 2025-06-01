using FUNewsManagementSystem.Common;
using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Middlewares;
using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Controllers
{
    [AllowAnonymous]
    [Route("odata/NewsArticles")]
    public class ArticleOdataController : ODataController
    {
        private readonly AppDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ArticleOdataController(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        public IQueryable<NewsArticle> Get()
        {
            return dbContext.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.SystemAccount)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.NewsStatus == "ACTIVE");
        }

        [HttpGet("{id}")]
        [EnableQuery(MaxTop = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        public async Task<IActionResult> Get(int id)
        {
            var article = await dbContext.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.SystemAccount)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync(n => n.NewsArticleID == id && n.NewsStatus == "ACTIVE");
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Put([FromBody] NewsArticleUpdateRequest updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingArticle = await dbContext.NewsArticles
                .Include(n => n.SystemAccount)
                .FirstOrDefaultAsync(n => n.NewsArticleID == updateDto.NewsArticleID);

            if (existingArticle == null)
            {
                return NotFound();
            }

            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var accountId = int.Parse(accountIdClaim);
            var user = await dbContext.SystemAccounts
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.AccountId == accountId);

            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }

            if (existingArticle.CreatedByID != accountId &&
                user.Role?.RoleName != DefinitionRole.ADMIN &&
                user.Role?.RoleName != DefinitionRole.STAFF)
            {
                throw new AppException(ErrorCode.FORBIDDEN);
            }

            if (updateDto.CategoryID != existingArticle.CategoryId)
            {
                var category = await dbContext.Categories.FindAsync(updateDto.CategoryID);
                if (category == null)
                {
                    throw new AppException(ErrorCode.CATEGORY_NOT_FOUND);
                }
            }

            existingArticle.NewsTitle = updateDto.NewsTitle;
            existingArticle.Headline = updateDto.Headline;
            existingArticle.NewsContent = updateDto.NewsContent;
            existingArticle.NewsSource = updateDto.NewsSource;
            existingArticle.CategoryId = updateDto.CategoryID;
            existingArticle.ImageUrl = updateDto.ImageUrl;
            existingArticle.NewsStatus = updateDto.NewsStatus ?? existingArticle.NewsStatus;
            existingArticle.ModifiedDate = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return Updated(existingArticle);
        }

    }

}