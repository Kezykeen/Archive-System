using archivesystemDomain.Interfaces;
using System.Threading.Tasks;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IFileRepo FileRepo { get; }
        


        public UnitOfWork(
            ApplicationDbContext context,
            IFileRepo fileRepo
           
            )
        {
            _context = context;
            FileRepo = fileRepo;
        }


        public int Save()
        {
            return _context.SaveChanges();
        }
        
        public async Task<int> SaveAsync()
        {
           return await _context.SaveChangesAsync();
           
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}