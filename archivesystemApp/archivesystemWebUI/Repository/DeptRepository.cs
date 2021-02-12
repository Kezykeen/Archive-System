using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;

namespace archivesystemWebUI.Repository
{
    public class DeptRepository : Repository<Department>, IDeptRepository
    {
        private readonly ApplicationDbContext _context;

        public DeptRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

       

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}