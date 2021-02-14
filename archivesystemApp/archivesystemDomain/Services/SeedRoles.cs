using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Configuration;
using archivesystemDomain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace archivesystemDomain.Services
{
    public static class SeedRoles
    {
        private static HttpContext Context => HttpContext.Current;
        private static ApplicationRoleManager RoleManager 
            => Context.GetOwinContext().GetUserManager<ApplicationRoleManager>();
        public static void EnsurePopulated()
        {
           
            Create("Admin");
            Create("Manager");
            Create("HR");
            
        }
        private static void Create(string name)
        {
            if (RoleManager.RoleExists(name)) return;
            var role = new ApplicationRole { Name = name };
            RoleManager.Create(role);
        }

    }
}