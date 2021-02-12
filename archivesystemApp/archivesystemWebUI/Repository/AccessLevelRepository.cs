using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Models;
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


        public IEnumerable<AccessLevel> AccessLevels { get { return _context.AccessLevels.ToList(); } }

        public AccessLevel GetAccessLevel(int levelNo)
        {
            var accessLevel = _context.AccessLevels.SingleOrDefault(m => m.Level == levelNo);
            return accessLevel;
        }
        public void CreateAccess(AccessLevel newAccess)
        {
            _context.AccessLevels.Add(newAccess);
        }
    }
}