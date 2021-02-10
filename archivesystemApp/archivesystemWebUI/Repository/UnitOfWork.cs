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

        public UnitOfWork(ApplicationDbContext dbContext, IEmployeeRepository employeeRepository )
        {
            _context = dbContext;
            EmployeeRepo = employeeRepository;
           
        }

        public IEmployeeRepository EmployeeRepo { get; }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
           
        }
    }
}