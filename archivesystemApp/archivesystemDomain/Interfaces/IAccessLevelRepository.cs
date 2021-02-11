using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IAccessLevelRepository
    {
        IEnumerable<AccessLevel> AccessLevels { get; } 
        void CreateAccess(AccessLevel newAccess);
    }
}
