using FUNewsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories
{
    public class RoleRepository
    {

        private readonly AppDbContext dbContext;
        public RoleRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task Save(Role role)
        {
            await dbContext.Roles.AddAsync(role);
            await dbContext.SaveChangesAsync();
        }
        public async Task<Role?> FindByName(string name)
        {
            return await dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == name);
        }
    }
}
