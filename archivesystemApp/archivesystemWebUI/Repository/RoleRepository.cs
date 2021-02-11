using archivesystemDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace archivesystemWebUI.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private HttpContext Context => HttpContext.Current;


        private ApplicationRoleManager RoleManager
        {
            get
            {
                return Context.GetOwinContext().Get<ApplicationRoleManager>();
            }
        } 

        //private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

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

        public async Task<IdentityResult> AddRole (string _roleName)
        {
            ApplicationRole role = new ApplicationRole(_roleName);
            IdentityResult result = await RoleManager.CreateAsync(role);
            return result;   
        }

        //public async Task<IdentiyResult> EditRole ()

        
    }
}