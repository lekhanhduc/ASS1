using FUNewsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories
{
    public class TagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetTagByIdAsync(int tagId)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.TagID == tagId);
        }

        public async Task<Tag?> GetTagByNameAsync(string tagName)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task AddTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(int tagId)
        {
            var tag = await GetTagByIdAsync(tagId);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
}