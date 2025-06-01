using FUNewsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories
{
    public class CategoryRepository
    {

        private readonly AppDbContext dbContext;

        public CategoryRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> FindByCategoryName(string categoryName)
        {
            return await dbContext.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == categoryName);
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await dbContext.Categories.ToListAsync();
        }

    }
}
