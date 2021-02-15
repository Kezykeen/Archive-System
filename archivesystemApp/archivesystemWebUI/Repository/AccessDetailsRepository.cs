using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class AccessDetailsRepository: Repository<AccessDetails>, IAccessDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public AccessDetailsRepository(ApplicationDbContext context)
           : base(context)
        {
            _context = context;
        }
    }
}