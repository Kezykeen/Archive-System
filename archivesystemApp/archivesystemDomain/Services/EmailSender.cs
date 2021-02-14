using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using archivesystemDomain.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace archivesystemDomain.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(Message message)
        {
            var apiKey = WebConfigurationManager.AppSettings["apiKey"];
            var user = WebConfigurationManager.AppSettings["user"];

            var mailClient = new SendGridClient(apiKey);

            var email = new SendGridMessage()
            {
                From = new EmailAddress("Demo@Domain.com", user),
                Subject = message.Subject,
                PlainTextContent = message.Body,
                HtmlContent = message.Body
            };

            email.AddTo(new EmailAddress(message.Destination));
            email.SetClickTracking(false, false);
            await mailClient.SendEmailAsync(email);

        }
    }
}
