using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace archivesystemDomain.Services
{
    public static class AccessCodeGenerator
    {
        public static string NewCode(string staffId)
        {
            var guid = Guid.NewGuid().ToString("N").Substring(0, 6);
            var accessCode = guid + staffId;
            return accessCode;
        }

        public static string HashCode(string code)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            string codeHash = BCrypt.Net.BCrypt.HashPassword(code, salt);
            return codeHash;
        }

        public static bool VerifyCode(string code, string hashedcode)
        {
            bool verified = BCrypt.Net.BCrypt.Verify(code, hashedcode);
            return verified;
        }

    }
}
