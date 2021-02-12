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
        public IEmployeeRepository EmployeeRepo { get; }
        public IDeptRepository DeptRepo { get; }
    


        public UnitOfWork(
            ApplicationDbContext context,
            IEmployeeRepository employeeRepo,
            IDeptRepository deptRepo
            )
        {
            _context = context;
            DeptRepo = deptRepo;
            EmployeeRepo = employeeRepo;
          
           
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