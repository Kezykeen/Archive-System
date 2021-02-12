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

        public UnitOfWork(ApplicationDbContext context,IRoleRepository roleRepository )

        {
            _context = context;
            EmployeeRepo = new EmployeeRepository(context);
            AccessLevelRepo = new AccessLevelRepository(context);
            RoleRepo = roleRepository;           
        }

        public IEmployeeRepository EmployeeRepo { get; }
        public IAccessLevelRepository AccessLevelRepo { get; }
        public IRoleRepository RoleRepo { get; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
           
        }

        public void Save()
        {
            _context.SaveChanges();

        }
    }
}