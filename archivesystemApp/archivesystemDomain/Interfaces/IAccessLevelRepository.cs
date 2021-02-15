using archivesystemDomain.Entities;
using System.Collections.Generic;

namespace archivesystemDomain.Interfaces
{
    public interface IAccessLevelRepository : IRepository<AccessLevel>
    {
        AccessLevel GetByLevel(string Level);
    }
}
