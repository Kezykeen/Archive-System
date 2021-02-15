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


        public async Task SendEmailAsync(string destination, string subject, string body)
        {
            var apiKey = WebConfigurationManager.AppSettings["apiKey"];
            var user = WebConfigurationManager.AppSettings["user"];

            var mailClient = new SendGridClient(apiKey);

            var email = new SendGridMessage()
            {
                From = new EmailAddress("tochukwuchinedu21@gmail.com", user),
                Subject = subject,
                PlainTextContent = body,
                HtmlContent = body
            };

            email.AddTo(new EmailAddress(destination));
            email.SetClickTracking(false, false);
            await mailClient.SendEmailAsync(email);
        }
    }
}