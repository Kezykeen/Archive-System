using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;

namespace archivesystemDomain.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly ITokenRepo _tokenRepo;
      

        public TokenGenerator(ITokenRepo tokenRepo)
        {
            _tokenRepo = tokenRepo;
        }
        public  string Code(int userId)
        {

            var existingTokens = _tokenRepo.Find(t => t.AppUserId == userId);
            if (existingTokens != null)
            {
                _tokenRepo.RemoveRange(existingTokens);

            }

            var token = new Token
            {
                Code = Guid.NewGuid().ToString("N"),
                AppUserId = userId,
                Expire = DateTime.Now.AddDays(3)
            };
            _tokenRepo.Add(token);
            return token.Code;
        }
    }
}
