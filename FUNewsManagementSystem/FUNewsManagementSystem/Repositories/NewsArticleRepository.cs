using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Middlewares;
using FUNewsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories
{
    public class NewsArticleRepository
    {
        private readonly AppDbContext dbContext;

        public NewsArticleRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<NewsArticle> CreateNewsArticleAsync(NewsArticle newsArticle)
        {
            dbContext.NewsArticles.Add(newsArticle);
            await dbContext.SaveChangesAsync();
            return newsArticle;
        }

        public async Task<NewsArticle?> GetNewsArticleByIdAsync(int id)
        {
            return await dbContext.NewsArticles
                .Include(na => na.Category)
                .Include(na => na.SystemAccount)
                .Include(na => na.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync(na => na.NewsArticleID == id);
        }

        public async Task<List<NewsArticle>> GetAll(int page, int size)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 10;

            var articles = await dbContext.NewsArticles
                .Include(na => na.Category)
                .Include(na => na.SystemAccount) 
                .Include(na => na.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderBy(na => na.CreatedDate)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return articles;
        }

        public async Task<long> TotalElements()
        {
            return await dbContext.NewsArticles.LongCountAsync();
        }

        public async Task<PageResponse<NewsArticleDetailResponse>> GetNewsArticlesByUserLogin(int page, int size, int userId)
        {
            if (page < 1) page = 1;
            if (size < 1) size = 10;
            var articles = await dbContext.NewsArticles
                .Where(na => na.CreatedByID == userId)
                .Include(na => na.Category)
                .Include(na => na.SystemAccount)
                .Include(na => na.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .OrderBy(na => na.CreatedDate)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var totalElements = await dbContext.NewsArticles.CountAsync(na => na.CreatedByID == userId);
            var articleDtos = articles.Select(na => new NewsArticleDetailResponse
            {
                NewsArticleID = na.NewsArticleID,
                NewsTitle = na.NewsTitle,
                Headline = na.Headline,
                CreatedDate = na.CreatedDate,
                NewsContent = na.NewsContent,
                NewsSource = na.NewsSource,
                CategoryName = na.Category != null ? na.Category.CategoryName : null,
                Tags = na.NewsTags.Select(nt => nt.Tag.TagName).ToList(),
                ImageUrl = na.ImageUrl ?? string.Empty,
                NewsStatus = na.NewsStatus,
                CreatedBy = na.SystemAccount.AccountName != null ? na.SystemAccount.AccountName : "Unknown"
            }).ToList();

            return new PageResponse<NewsArticleDetailResponse>
            {
                CurrentPage = page,
                PageSize = size,
                TotalElements = totalElements,
                TotalPages = (int)Math.Ceiling((double)totalElements / size),
                Data = articleDtos
            };
        }



        public async Task AddTagsToArticleAsync(int newsArticleId, IEnumerable<int> tagIds)
        {
            foreach (var tagId in tagIds)
            {
                var newsArticleTag = new NewsTag
                {
                    NewsArticleID = newsArticleId,
                    TagID = tagId
                };
                await dbContext.NewsTags.AddAsync(newsArticleTag);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveTagsFromArticleAsync(int newsArticleId)
        {
            var tags = await dbContext.NewsTags
                .Where(nt => nt.NewsArticleID == newsArticleId)
                .ToListAsync();

            if (tags.Any())
            {
                dbContext.NewsTags.RemoveRange(tags);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteNewsArticleAsync(int id)
        {
            var newsArticle = await dbContext.NewsArticles
                .FirstOrDefaultAsync(n => n.NewsArticleID == id);

            if (newsArticle == null)
            {
                throw new AppException(ErrorCode.NEWS_ARTICLE_NOT_FOUND);
            }

            dbContext.NewsArticles.Remove(newsArticle);
            await dbContext.SaveChangesAsync();
        }

    }
}
