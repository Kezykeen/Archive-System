using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;

namespace archivesystemWebUI.Repository
{
    public class ApplicationRepo : Repository<Application>, IApplicationRepo
    {
        private readonly ApplicationDbContext _context;


        public ApplicationRepo(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }


        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}