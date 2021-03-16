using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemWebUI.Interfaces
{
    public interface IAccessLevelService
    {
        bool CheckLevel(string Level);
        Task Create(AccessLevel newAccess);
        IEnumerable<AccessLevel> GetAll();
        AccessLevel GetById(int id);
        Task Update(AccessLevel accessLevel);
    }
}
