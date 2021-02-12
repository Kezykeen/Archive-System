using archivesystemDomain.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Interfaces
{
    public interface IRoleService
    {
        IEnumerable<ApplicationRole> GetAllRoles();
        void DeleteRole(string roleId);
        Task<IdentityResult> AddRole(string _roleName);

        Task<IdentityResult> EditRole(string oldName, string newName);
    }
}
