using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Contra.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public EmailOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task SendConfirmEmailAsync(string email, string name, string url)
        {
            var client = new SendGridClient(Options.SendGridKey);
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("no-reply@contra.live", Options.SendGridUser));
            msg.AddTo(new EmailAddress(email));
            msg.SetTemplateId("d-3cd5f6605a9442a2b975161e49e95912");

            var templateData = new ConfirmEmailTemplateData
            {
                User = name,
                Code = url
            };

            msg.SetTemplateData(templateData);
            msg.SetClickTracking(false, false);
            return client.SendEmailAsync(msg);
        }

        private class ConfirmEmailTemplateData
        {
            [JsonProperty("user")]
            public string User { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("no-reply@contra.live", Options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}