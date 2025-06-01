namespace FUNewsManagementSystem.Dtos.Response
{
    public class UserCreationResponse
    {
        public string Email { get; set; }
        public string AccountName { get; set; }

        public string? Role { get;set; }

        public UserCreationResponse(string Email,  string AccountName, string Role)
        {
            this.Email = Email;
            this.AccountName = AccountName;
            this.Role = Role;
        }
    }
}
