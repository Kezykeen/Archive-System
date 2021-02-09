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
        private readonly ApplicationDbContext _Context;

        public UnitOfWork(IStaffRepository staffRepository, ApplicationDbContext dbContext)
        {
            StaffRepo = staffRepository;
            _Context = dbContext;
        }

        public IStaffRepository StaffRepo { get; }

        public void Save()
        {
            _Context.SaveChangesAsync();
           
        }
    }
}