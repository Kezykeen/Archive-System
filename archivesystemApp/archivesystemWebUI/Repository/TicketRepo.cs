using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;

namespace archivesystemWebUI.Repository
{
    public class TicketRepo : Repository<Ticket>, ITicketRepo
    {
        private readonly ApplicationDbContext _context;


        public TicketRepo(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }


        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}