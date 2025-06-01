using FUNewsManagementSystem.Common;
using FUNewsManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext dbContext, PasswordHasher<SystemAccount> passwordHasher)
        {
            await dbContext.Database.MigrateAsync();

            if (!await dbContext.Roles.AnyAsync(r => r.RoleName == DefinitionRole.ADMIN))
            {
                var adminRole = new Role
                {
                    RoleName = DefinitionRole.ADMIN,
                    Description = "Administrator role with full permissions"
                };
                dbContext.Roles.Add(adminRole);
            }

            if (!await dbContext.Roles.AnyAsync(r => r.RoleName == DefinitionRole.STAFF))
            {
                var staffRole = new Role
                {
                    RoleName = DefinitionRole.STAFF,
                    Description = "Staff role with limited permissions"
                };
                dbContext.Roles.Add(staffRole);
            }

            await dbContext.SaveChangesAsync();

            var adminRoleId = await dbContext.Roles
                .Where(r => r.RoleName == DefinitionRole.ADMIN)
                .Select(r => r.RoleId)
                .FirstOrDefaultAsync();
            var staffRoleId = await dbContext.Roles
                .Where(r => r.RoleName == DefinitionRole.STAFF)
                .Select(r => r.RoleId)
                .FirstOrDefaultAsync();

            if (!await dbContext.SystemAccounts.AnyAsync(a => a.Email == "admin@gmail.com"))
            {
                var adminUser = new SystemAccount
                {
                    AccountName = "Admin User",
                    Email = "admin@gmail.com",
                    RoleId = adminRoleId,
                    userStatus = UserStatus.ACTIVE
                };
                adminUser.Password = passwordHasher.HashPassword(adminUser, "123456");
                dbContext.SystemAccounts.Add(adminUser);
            }

            if (!await dbContext.SystemAccounts.AnyAsync(a => a.Email == "staff@gmail.com"))
            {
                var staffUser = new SystemAccount
                {
                    AccountName = "Staff User",
                    Email = "staff@gmail.com",
                    RoleId = staffRoleId,
                    userStatus = UserStatus.ACTIVE
                };
                staffUser.Password = passwordHasher.HashPassword(staffUser, "123456");
                dbContext.SystemAccounts.Add(staffUser);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}