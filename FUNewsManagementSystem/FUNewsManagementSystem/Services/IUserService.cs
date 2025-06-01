using FUNewsManagementSystem.Dtos.Request;
using FUNewsManagementSystem.Dtos.Response;

namespace FUNewsManagementSystem.Services
{
    public interface IUserService
    {
        Task<UserCreationResponse> CreateUser(UserCreationRequest request);
        Task<BannedAccountResponse> UpdateStatusUser(BannedAccountRequest request);
        Task<PageResponse<UserDetailResponse>> GetAllUsers(int page, int size);

        Task<UserDetailResponse> GetUserLogin();

        Task ChangePassword(ChangePasswordRequest request);

    }
}
