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

        public UnitOfWork(ApplicationDbContext context )
        {
            _context = context;
            EmployeeRepo = new EmployeeRepository(context);
            RoleRepo = new RoleRepository();
           
        }

        public IEmployeeRepository EmployeeRepo { get; }
        public IRoleRepository RoleRepo { get; }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
           
        }
    }
}