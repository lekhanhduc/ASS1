using FUNewsManagementSystem.Common;
using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Middlewares;
using FUNewsManagementSystem.Models;
using FUNewsManagementSystem.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FUNewsManagementSystem.Services.Impl
{
    public class UserService : IUserService
    {

        private readonly SystemAccountRepository systemAccountRepository;
        private readonly RoleRepository roleRepository;
        private readonly PasswordHasher<SystemAccount> passwordHasher;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(SystemAccountRepository systemAccountRepository, RoleRepository roleRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.systemAccountRepository = systemAccountRepository;
            this.roleRepository = roleRepository;
            this.passwordHasher = new PasswordHasher<SystemAccount>();
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task ChangePassword(ChangePasswordRequest request)
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
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, request.OldPassword.Trim());

            if (result == PasswordVerificationResult.Failed)
            {
                throw new AppException(ErrorCode.PASSWORD_INCORRECT);
            }

            user.Password = passwordHasher.HashPassword(user, request.NewPassword.Trim());
            await systemAccountRepository.Update(user);
        }

        public async Task<UserCreationResponse> CreateUser(UserCreationRequest request)
        {
            var user = await systemAccountRepository.FindByEmail(request.Email);

            if(user != null)
            {
                throw new AppException(ErrorCode.USER_EXISTED);
            }

            var role = await roleRepository.FindByName(DefinitionRole.USER);
            if(role == null)
            {
                role = new Role
                {
                    RoleName = DefinitionRole.USER,
                    Description = "Default user role"
                };
                await roleRepository.Save(role);
            }

            SystemAccount account = new SystemAccount
            {
                AccountName = request.AccountName,
                Email = request.Email,
                Role = role,
            };
            account.Password = passwordHasher.HashPassword(account, request.Password.Trim());

            await systemAccountRepository.Save(account);

            return new UserCreationResponse(account.Email, account.AccountName, role.RoleName);
        }

        public async Task<PageResponse<UserDetailResponse>> GetAllUsers(int page, int size)
        {
            var users = await systemAccountRepository.GetAllUsers(page, size);
            var totalElements = await systemAccountRepository.TotalElements();

            var userDtos = users.Select(u => new UserDetailResponse
            {
                AccountId = u.AccountId,
                AccountName = u.AccountName,
                Email = u.Email,
                RoleName = u.Role.RoleName,
                UserStatus = u.userStatus.ToString(),
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalElements / size);
            var response = new PageResponse<UserDetailResponse>
            {
                CurrentPage = page,
                PageSize = size,
                TotalPages = totalPages,
                TotalElements = totalElements,
                Data = userDtos
            };

            return response;
        }

        public async Task<UserDetailResponse> GetUserLogin()
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

            return new UserDetailResponse
            {
                AccountId = user.AccountId,
                AccountName = user.AccountName,
                Email = user.Email,
                RoleName = user.Role.RoleName,
                UserStatus = user.userStatus.ToString(),
            };
        }

        public async Task<BannedAccountResponse> UpdateStatusUser(BannedAccountRequest request)
        {
            var user = await systemAccountRepository.FindById(request.UserId);
            if (user == null)
            {
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }

            if (request.Action == "BAN")
            {
                user.userStatus = UserStatus.BANNED;
            }
            else if (request.Action == "UNBAN")
            {
                user.userStatus = UserStatus.ACTIVE;
            }

            await systemAccountRepository.Update(user);

            return new BannedAccountResponse
            {
                UserId = user.AccountId,
                Reason = request.Reason,
                BannedAt = DateTime.UtcNow,
            };

        }
    }
}
