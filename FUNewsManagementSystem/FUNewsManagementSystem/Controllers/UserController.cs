using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse<UserCreationResponse>> CreateUser([FromBody] UserCreationRequest request)
        {
            var users = await userService.CreateUser(request);

            return new ApiResponse<UserCreationResponse>(
                code: 201,
                message: "Created user",
                result: users
            );
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<ApiResponse<UserDetailResponse>> GetUserInfo()
        {
            var user = await userService.GetUserLogin();
            return new ApiResponse<UserDetailResponse>(
                code: 200,
                message: "Retrieved user information",
                result: user
            );
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<ApiResponse<UserDetailResponse>> UpdatePassword([FromBody] ChangePasswordRequest request)
        {
            await userService.ChangePassword(request);
            return new ApiResponse<UserDetailResponse>(
                code: 200,
                message: "Updated user password"
            );
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<PageResponse<UserDetailResponse>>> GetAllUsers(
            [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var users = await userService.GetAllUsers(page, size);
            return new ApiResponse<PageResponse<UserDetailResponse>>(
                code: 200,
                message: "Retrieved all users",
                result: users
            );
        }

        [HttpPut("status")]
        public async Task<ApiResponse<BannedAccountResponse>> UpdateStatusUser([FromBody] BannedAccountRequest request)
        {
            var response = await userService.UpdateStatusUser(request);
            return new ApiResponse<BannedAccountResponse>(
                code: 200,
                message: "Updated user status",
                result: response
            );
        }

    }
}
