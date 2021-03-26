using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using BCrypt.Net;

namespace archivesystemDomain.Services
{
    public class AccessCodeGenerator : IAccessCodeGenerator
    {

        private const string add = "add";
        private const string update = "update";
        private readonly IEmailSender _emailSender;

        public AccessCodeGenerator(IEmailSender iemailsender)
        {
            _emailSender = iemailsender;
        }

        public AccessCodeGenerator()
        {
        }

        public string NewCode(string staffId)
        {
            var guid = Guid.NewGuid().ToString("N").Substring(0, 6);
            var accessCode = guid + staffId;
            return accessCode;
        }

        public string HashCode(string code)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var codeHash = BCrypt.Net.BCrypt.HashPassword(code, salt);
            return codeHash;
        }

        public bool VerifyCode(string code, string hashedcode)
        {
            var verified = BCrypt.Net.BCrypt.Verify(code, hashedcode);
            return verified;
        }

        public async Task<string> GenerateCode(AppUser user, string method)
        {
            var accessCode = NewCode(user.TagId);

            switch (method)
            {
                case add:
                    await _emailSender.SendEmailAsync(
                                                user.Email, "Access Code",
                                                $"Hello {user.Name},\nYour access code is:\n<strong>{accessCode}</strong>.\nThis is confidential. Do not share with anyone.");
                    return HashCode(accessCode);

                case update:
                    await _emailSender.SendEmailAsync(
                                                 user.Email, "Access Code (Updated)",
                                                 $"Hello {user.Name},\nYour new access code is:\n<strong>{accessCode}</strong>.\nThis is confidential. Do not share with anyone.");
                    return HashCode(accessCode);

                default:
                    return HashCode(accessCode);
            }
        }

    }
}
