using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;

namespace FUNewsManagementSystem.Services
{
    public interface IAuthService
    {
        Task<SignInResponse> SignIn(SignInRequest request);
        Task<SignInResponse> SignInWithGoogle(string code);

        Task<IntrospectResponse> VerificationToken(IntrospectRequest request);
        Task<SignInResponse> RefreshToken();
        Task SignOut();
    }
}
