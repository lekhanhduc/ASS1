using System.Net;
using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementSystem.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ApiResponse<SignInResponse>> Login([FromBody] SignInRequest request)
        {
            var response = await authService.SignIn(request);
            return new ApiResponse<SignInResponse>(200, "Login success", response);
        }

        [HttpPost("outbound")]
        [AllowAnonymous]
        public async Task<ApiResponse<SignInResponse>> SigInGoogle([FromQuery] string code)
        {
            var result = await authService.SignInWithGoogle(code);

            return new ApiResponse<SignInResponse>
            {
                code = ((int)HttpStatusCode.OK),
                message = "SignIn Google Successfully",
                result = result
            };
        }

        [HttpPost("introspect")]
        public async Task<ApiResponse<IntrospectResponse>> Introspect([FromBody] IntrospectRequest request)
        {
            var result = await authService.VerificationToken(request);
            return new ApiResponse<IntrospectResponse>
            {
                code = 200,
                message = "Introspect Successfully",
                result = result
            };
        }

    }
}
