using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Middlewares;
using FUNewsManagementSystem.Repositories;
using FUNewsManagementSystem.Models;
using FUNewsManagementSystem.Common;

namespace FUNewsManagementSystem.Services.Impl
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SystemAccountRepository systemAccountRepository;
        private readonly NewsArticleRepository newsArticleRepository;
        private readonly CategoryRepository categoryRepository;
        private readonly TagRepository tagRepository;
        private readonly CloudinaryService cloudinaryService;

        public NewsArticleService(
            IHttpContextAccessor httpContextAccessor,
            SystemAccountRepository systemAccountRepository,
            NewsArticleRepository newsArticleRepository,
            CategoryRepository categoryRepository,
            TagRepository tagRepository,
            CloudinaryService cloudinaryService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.systemAccountRepository = systemAccountRepository;
            this.newsArticleRepository = newsArticleRepository;
            this.categoryRepository = categoryRepository;
            this.tagRepository = tagRepository;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task<NewsArticleCreationResponse> CreateNewsArticleAsync(NewsArticleCreationRequest request)
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

            var category = await categoryRepository.GetCategoryByIdAsync(request.CategoryId);
            if (category == null)
            {
                throw new AppException(ErrorCode.CATEGORY_NOT_FOUND);
            }

            string? imageUrl = null;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                using var stream = request.ImageFile.OpenReadStream();
                imageUrl = await cloudinaryService.UploadImageAsync(stream, request.ImageFile.FileName);
            }

            var newsArticle = new NewsArticle
            {
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                CreatedByID = accountId,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                NewsStatus = "ACTIVE",
                ImageUrl = imageUrl
            };

            var createdArticle = await newsArticleRepository.CreateNewsArticleAsync(newsArticle);

            var tagEntities = new List<Tag>();
            if (request.Tags != null && request.Tags.Any())
            {
                foreach (var tagName in request.Tags.Where(t => !string.IsNullOrWhiteSpace(t)))
                {
                    var trimmedTagName = tagName.Trim();
                    var existingTag = await tagRepository.GetTagByNameAsync(trimmedTagName);
                    if (existingTag == null)
                    {
                        var newTag = new Tag
                        {
                            TagName = trimmedTagName,
                            Note = $"Auto-created for article: {request.NewsTitle}"
                        };
                        await tagRepository.AddTagAsync(newTag);
                        existingTag = newTag;
                    }
                    tagEntities.Add(existingTag);
                }
                await newsArticleRepository.AddTagsToArticleAsync(createdArticle.NewsArticleID, tagEntities.Select(t => t.TagID));
            }

            return new NewsArticleCreationResponse
            {
                NewsArticleID = createdArticle.NewsArticleID,
                NewsTitle = createdArticle.NewsTitle,
                Headline = createdArticle.Headline,
                NewsContent = createdArticle.NewsContent,
                NewsSource = createdArticle.NewsSource,
                CategoryName = category.CategoryName,
                Tags = tagEntities.Select(t => t.TagName).ToList(),
                ImageUrl = createdArticle.ImageUrl,
                CreatedBy = user.AccountName,
                CreatedDate = createdArticle.CreatedDate,
                NewsStatus = createdArticle.NewsStatus,
            };
        }

        public async Task DeleteNewsArticleAsync(int id)
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

            // Kiểm tra quyền của người dùng
            if (user.Role?.RoleName != DefinitionRole.STAFF && user.Role?.RoleName != DefinitionRole.ADMIN)
            {
                throw new AppException(ErrorCode.FORBIDDEN);
            }

            var newsArticle = await newsArticleRepository.GetNewsArticleByIdAsync(id);
            if (newsArticle == null)
            {
                throw new AppException(ErrorCode.NEWS_ARTICLE_NOT_FOUND);
            }

            if (newsArticle.CreatedByID != accountId)
            {
                throw new AppException(ErrorCode.FORBIDDEN);
            }

            await newsArticleRepository.RemoveTagsFromArticleAsync(id);
            await newsArticleRepository.DeleteNewsArticleAsync(id);
        }

        public async Task<PageResponse<NewsArticleDetailResponse>> GetAllNewsArticlesAsync(int page, int size)
        {
            var articles = await newsArticleRepository.GetAll(page, size);
            var totalElements = await newsArticleRepository.TotalElements();

            var articleDtos = articles.Select(na => new NewsArticleDetailResponse
            {
                NewsArticleID = na.NewsArticleID,
                NewsTitle = na.NewsTitle,
                Headline = na.Headline,
                CreatedDate = na.CreatedDate,
                NewsContent = na.NewsContent,
                NewsSource = na.NewsSource,
                CategoryName = na.Category != null ? na.Category.CategoryName : null,
                Tags = na.NewsTags.Select(nt => nt.Tag.TagName).ToList(), // Lấy TagName từ NewsTag
                ImageUrl = na.ImageUrl ?? string.Empty,
                NewsStatus = na.NewsStatus,
                CreatedBy = na.SystemAccount.AccountName
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalElements / size);


            var response = new PageResponse<NewsArticleDetailResponse>
            {
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalElements = totalElements,
                Data = articleDtos
            };

            return response;
        }

        public async Task<NewsArticleDetailResponse?> GetNewsArticleByIdAsync(int id)
        {
            var newsArticle = await newsArticleRepository.GetNewsArticleByIdAsync(id);
            if (newsArticle == null)
            {
                throw new AppException(ErrorCode.NEWS_ARTICLE_NOT_FOUND);
            }

            return new NewsArticleDetailResponse
            {
                NewsArticleID = newsArticle.NewsArticleID,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CategoryName = newsArticle.Category?.CategoryName,
                Tags = newsArticle.NewsTags.Select(nt => nt.Tag.TagName).ToList(),
                ImageUrl = newsArticle.ImageUrl ?? string.Empty,
                NewsStatus = newsArticle.NewsStatus,
                CreatedBy = newsArticle.SystemAccount?.AccountName
            };
        }

        public async Task<PageResponse<NewsArticleDetailResponse>> GetNewsArticlesByUserLogin(int page, int size)
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

            var articles = await newsArticleRepository.GetNewsArticlesByUserLogin(page, size, accountId);
            return articles;
        }

    }
}