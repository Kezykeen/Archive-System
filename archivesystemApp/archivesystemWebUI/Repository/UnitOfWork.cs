using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IStaffRepository staffRepository)
        {
            StaffRepo = staffRepository;
        }
        public IStaffRepository StaffRepo { get; }

    }
}