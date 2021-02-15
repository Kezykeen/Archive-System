using archivesystemDomain.Entities;
using System.Collections.Generic;

namespace archivesystemDomain.Interfaces
{
    public interface IAccessLevelRepository
    {
        IEnumerable<AccessLevel> AccessLevels { get; } 
        void CreateAccess(AccessLevel newAccess);
    }
}
