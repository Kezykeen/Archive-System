using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace archivesystemDomain.Services
{
    public class RoleService : IRoleService
    {
        private static HttpContext  Context => HttpContext.Current;

        
        private ApplicationRoleManager RoleManager => Context.GetOwinContext().GetUserManager<ApplicationRoleManager>();
        private ApplicationUserManager UserManager => Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

        public IEnumerable<ApplicationRole> GetAllRoles()
        {
            return RoleManager.Roles.ToList();
        }

        public void DeleteRole(string roleId)
        {
            var role = RoleManager.FindById(roleId);
            if (role != null)
                RoleManager.Delete(role);
        }

        public async Task<IdentityResult> AddRole (string roleName)
        {
            ApplicationRole role = new ApplicationRole { Name=roleName,CreatedAt=DateTime.Now,UpDatedAt=DateTime.Now};

            IdentityResult result = await RoleManager.CreateAsync(role);
            return result;   
        }

        public async Task<IdentityResult> EditRole(string oldName, string newName)
        {
           ApplicationRole role=  await RoleManager.FindByNameAsync(oldName);
           role.Name = newName;
           role.UpDatedAt = DateTime.Now;
           var result =await RoleManager.UpdateAsync(role);
           return result;
        }

        public async Task<ICollection<string>> GetUserIdsOfUsersInRole(string roleName)
        {
            var role=await RoleManager.FindByNameAsync(roleName);
            var identityUserRoleObjs =role.Users;
            List<string> userIds = identityUserRoleObjs.Select(x => x.UserId).ToList();
            return userIds;

        }
    }
}