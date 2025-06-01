namespace FUNewsManagementSystem.Dtos.Response
{
    public class SignInResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public string UserType { get; set; }
        public string TokenType { get; set; }

        public SignInResponse(string AccessToken, string RefreshToken, string UserType,  string TokenType)
        {
            this.AccessToken = AccessToken;
            this.RefreshToken = RefreshToken;
            this.UserType = UserType;
            this.TokenType = TokenType;
        }
    }
}
