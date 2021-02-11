using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class AccessLevelRepository: IAccessLevelRepository
    {
        private readonly ApplicationDbContext _context;

        public AccessLevelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateAccess()
        {
           
        }
    }
}