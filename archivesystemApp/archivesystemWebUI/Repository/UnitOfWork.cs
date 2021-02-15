using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ITokenRepo TokenRepo { get; }
        public IEmployeeRepository EmployeeRepo { get; }
        public IDeptRepository DeptRepo { get; }
        public IAccessLevelRepository AccessLevelRepo { get; }



        public UnitOfWork(
            ApplicationDbContext context,
            IEmployeeRepository employeeRepo,
            IDeptRepository deptRepo,
            IAccessLevelRepository accessLevelRepo,
            ITokenRepo tokenRepo
            )
        {
            _context = context;
            TokenRepo = tokenRepo;
            DeptRepo = deptRepo;
            AccessLevelRepo = accessLevelRepo;
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