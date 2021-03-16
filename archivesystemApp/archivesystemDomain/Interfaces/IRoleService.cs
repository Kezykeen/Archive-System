using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        IdentityResult AddToRole(string userId, string roleId);
        Task<IdentityResult> EditRole(string oldName, string newName);
        Task<ICollection<string>> GetUserIdsOfUsersInRole(string name);
        IEnumerable<RoleMemberData> GetUsersData(ICollection<string> userIds);
        string GetEmployeeName(string userId);
        IdentityResult RemoveFromRole(string userId, string roleName);

    }

}
