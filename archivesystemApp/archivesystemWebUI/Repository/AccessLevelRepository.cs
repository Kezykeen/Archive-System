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
    public class AccessLevelRepository: Repository<AccessLevel>, IAccessLevelRepository
    {
        private readonly ApplicationDbContext _context;

        public AccessLevelRepository(ApplicationDbContext context)
            :base(context)
        {
            _context = context;
        }

        public AccessLevel GetByLevel(string Level)
        {
            var access = _context.AccessLevels.FirstOrDefault(m => m.Level == Level);
            return access;
        }
    }
}