using FUNewsManagementSystem.Common;
using FUNewsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.Repositories
{
    public class SystemAccountRepository
    {

        private readonly AppDbContext dbContext;

        public SystemAccountRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Save(SystemAccount account)
        {
            await dbContext.SystemAccounts.AddAsync(account);
            await dbContext.SaveChangesAsync();
        }

        public async Task Update(SystemAccount account)
        {
            dbContext.SystemAccounts.Update(account);
            await dbContext.SaveChangesAsync();
        }

        public async Task<SystemAccount?> FindByEmail(string email)
        {
            return await dbContext.SystemAccounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<SystemAccount?> FindById(int accountId)
        {
            return await dbContext.SystemAccounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<List<SystemAccount>> GetAllAccounts()
        {
            return await dbContext.SystemAccounts
                .Include(a => a.Role)
                .ToListAsync();
        }

        public async Task<List<SystemAccount>> GetAllUsers(int page, int size)
        {
            return await dbContext.SystemAccounts
                .Where(a => a.Role.RoleName == DefinitionRole.USER || a.Role.RoleName == DefinitionRole.STAFF)
                .Include(a => a.Role)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }

        public async Task<long> TotalElements()
        {
            return await dbContext.SystemAccounts
                .Where(a => a.Role.RoleName == DefinitionRole.USER || a.Role.RoleName == DefinitionRole.STAFF)
                .LongCountAsync();
        }

    }
}
