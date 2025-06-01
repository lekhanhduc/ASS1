namespace FUNewsManagementSystem.Services
{
    public interface IMailService
    {
        Task SendEmailVerification(string to, string name);
    }
}
