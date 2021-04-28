using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;

namespace archivesystemWebUI.Repository
{
    public class TokenRepository : Repository<Token>, ITokenRepo
    {
        private readonly ApplicationDbContext _context;

        public TokenRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }



        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}