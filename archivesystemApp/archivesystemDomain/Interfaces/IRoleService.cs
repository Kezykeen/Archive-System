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
        Task<IdentityResult> AddRole(string _roleName);
        IdentityResult AddToRole(string userId, string roleId);
        IdentityResult AddToRoleByEmail(string userEmail, string roleId);
        void DeleteRole(string roleId);
        Task<IdentityResult> EditRole(string oldName, string newName);
        IEnumerable<ApplicationRole> GetAllRoles();
        string GetEmployeeName(string userId);
        IEnumerable<RoleMemberData> GetUsersData(ICollection<string> userIds);
        Task<ICollection<string>> GetUserIdsOfUsersInRole(string name);
        IdentityResult RemoveFromRole(string userId, string roleName);

    }

}
