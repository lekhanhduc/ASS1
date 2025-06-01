using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using FUNewsManagementSystem.Common;
using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Middlewares;
using FUNewsManagementSystem.Models;
using FUNewsManagementSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FUNewsManagementSystem.Services.Impl
{
    public class AuthService : IAuthService
    {

        private readonly ILogger<AuthService> logger;
        private readonly IJwtService jwtService;
        private readonly PasswordHasher<SystemAccount> passwordHasher;
        private readonly SystemAccountRepository systemAccountRepository;
        private readonly RoleRepository roleRepository;
        private readonly GoogleAuthClient googleAuthClient;
        private readonly GoogleUserInfoClient googleUserInfoClient;
        private readonly IConfiguration _configuration;


        public AuthService(
            ILogger<AuthService> logger,
            IJwtService jwtService,
            SystemAccountRepository systemAccountRepository,
            RoleRepository roleRepository,
            GoogleAuthClient googleAuthClient,
            GoogleUserInfoClient googleUserInfoClient,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.jwtService = jwtService;
            this.passwordHasher = new PasswordHasher<SystemAccount>();
            this.systemAccountRepository = systemAccountRepository;
            this.roleRepository = roleRepository;
            this.googleAuthClient = googleAuthClient;
            this.googleUserInfoClient = googleUserInfoClient;
            _configuration = configuration;
        }


        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            logger.LogInformation("SignIn start ...");

            var user = await systemAccountRepository.FindByEmail(request.Email);

            if (user == null)
            {
                logger.LogError("SignIn Failed: User not found.");
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }

            if(user.userStatus != UserStatus.ACTIVE)
            {
                logger.LogError("SignIn Failed: User account is not active.");
                throw new AppException(ErrorCode.ACCOUNT_LOCKED);
            }

            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                logger.LogError("SignIn Failed: Invalid password.");
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var claims = new[]
            {
                new Claim("userId", user.AccountId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Authorities", user.Role.RoleName)
            };

            var accessToken = jwtService.GenerateAccessToken(claims);
            var refreshToken = jwtService.GenerateRefreshToken(claims);

            logger.LogInformation("SignIn success for userId: {UserId}", user.AccountId);

            return new SignInResponse(accessToken, refreshToken, user.Role.RoleName ,"Bearer");
        }

        public async Task<SignInResponse> SignInWithGoogle(string code)
        {
            var token = await googleAuthClient.ExchangeTokenAsync(code);
            var userInfo = await googleUserInfoClient.GetUserInfoAsync(token.AccessToken);

            var user = await systemAccountRepository.FindByEmail(userInfo.Email);

            if (user == null)
            {
                var role = await roleRepository.FindByName(DefinitionRole.USER);
                if (role == null)
                {
                    role = new Role();
                    role.RoleName = DefinitionRole.USER;
                    await roleRepository.Save(role);
                }
                user = new SystemAccount
                {
                    Email = userInfo.Email,
                    AccountName = userInfo.Name,
                    Role = role
                };
                await systemAccountRepository.Save(user);
            }

            var claims = new[]
               {
                    new Claim("userId", user.AccountName.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Authorities", user.Role.RoleName)
                };

            var accessToken = jwtService.GenerateAccessToken(claims);
            var refreshToken = jwtService.GenerateRefreshToken(claims);

            logger.LogInformation("SignIn Google success for userId: {UserId}", user.AccountId);

            return new SignInResponse(accessToken, refreshToken, user.Role.RoleName, "Bearer");
        }

        public async Task<IntrospectResponse> VerificationToken(IntrospectRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(request.Token, validationParameters, out _);
                var roleClaim = principal?.FindFirst("Authorities")?.Value;

                return new IntrospectResponse
                {
                    IsValid = true,
                    UserType = roleClaim ?? "Unknown"
                };
            }
            catch
            {
                return new IntrospectResponse
                {
                    IsValid = false,
                    UserType = "Invalid"
                };
            }
        }

        public Task<SignInResponse> RefreshToken()
        {
            throw new NotImplementedException();
        }

        public Task SignOut()
        {
            throw new NotImplementedException();
        }
    }
}