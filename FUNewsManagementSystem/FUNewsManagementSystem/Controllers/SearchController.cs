using FUNewsManagementSystem.Dtos.Response;
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
    public class SearchController : ODataController
    {
        private readonly AppDbContext dbContext;

        public SearchController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
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
    }

}