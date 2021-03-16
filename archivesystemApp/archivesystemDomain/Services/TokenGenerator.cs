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
        private readonly IUnitOfWork _unitOfWork;

        public TokenGenerator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public  string Code(int userId)
        {

            var existingTokens = _unitOfWork.TokenRepo.Find(t => t.AppUserId == userId);
            if (existingTokens != null)
            {
                _unitOfWork.TokenRepo.RemoveRange(existingTokens);

            }

            var token = new Token
            {
                Code = Guid.NewGuid().ToString("N"),
                AppUserId = userId,
                Expire = DateTime.Now.AddDays(3)
            };
            _unitOfWork.TokenRepo.Add(token);
            return token.Code;
        }
    }
}
