using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IAccessLevelRepository : IRepository<AccessLevel>
    {
        AccessLevel GetByLevel(string Level);
        void EditDetails(AccessLevel accessLevel);
    }
}
