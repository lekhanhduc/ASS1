using CloudinaryDotNet.Core;

namespace FUNewsManagementSystem.Dtos.Request
{
    public class UserCreationRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string AccountName { get; set; }

        public UserCreationRequest(string Email, string Password, string AccountName)
        {
            this.Email = Email;
            this.AccountName = AccountName;
            this.Password = Password;
        }
    }
}
