using SendGrid.Helpers.Mail;
using SendGrid;

namespace FUNewsManagementSystem.Services.Impl
{
    public class MailService: IMailService
    {
        private readonly string _sendGridApiKey;
        private readonly string _templateId;
        private readonly string _templateIdBuyCourse;
        private readonly ILogger<MailService> _logger;

        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            _sendGridApiKey = configuration["SendGrid:ApiKey"];
            _templateId = configuration["SendGrid:TemplateId"];
            _templateIdBuyCourse = configuration["SendGrid:TemplateIdBuyCourse"];
            _logger = logger;
        }

        public async Task SendEmailVerification(string to, string name)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("lekhanhduccc@gmail.com", "FUNewsManagementSystem");
            var toEmail = new EmailAddress(to);
            var msg = new SendGridMessage
            {
                TemplateId = _templateId,
                From = from,
                Subject = "Email Welcome for FUNewsManagementSystem"
            };

            msg.AddTo(toEmail);
            msg.SetTemplateData(new
            {
                name = name,
            });

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation("Email sent successfully to {Email}", to);
            }
            else
            {
                _logger.LogError("Failed to send email to {Email}. Status: {StatusCode} {Body}", to, response.StatusCode, response.Body);
            }
        }

    }
}
