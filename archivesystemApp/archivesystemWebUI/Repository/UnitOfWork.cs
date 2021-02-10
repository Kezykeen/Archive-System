using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext dbContext, IStaffRepository staffRepository )
        {
            _context = dbContext;
            StaffRepo = staffRepository;
           
        }

        public IStaffRepository StaffRepo { get; }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
           
        }
    }
}