using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using archivesystemDomain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace archivesystemDomain.Services
{
    
        public static class SeedAdmin
        {
            private static readonly string AdminUser = WebConfigurationManager.AppSettings["AdminUser"];
            private static readonly string AdminPassword = WebConfigurationManager.AppSettings["AdminPwd"];
            private static readonly string AdminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
            private static readonly string AdminPhone = WebConfigurationManager.AppSettings["AdminPhone"];

            public static async void EnsurePopulated()
            {
               var context = HttpContext.Current;

               var dbContext =  context.GetOwinContext().Get<ApplicationDbContext>();

               var userManager = context.GetOwinContext().GetUserManager<ApplicationUserManager>();
               var user = await userManager.FindByNameAsync(AdminUser);

               if (user != null) return;
               user = new ApplicationUser
                    {UserName = "Admin", Email = AdminEmail, PhoneNumber = AdminPhone, EmailConfirmed = true };
               var createUser = await userManager.CreateAsync(user, AdminPassword);

               if (createUser.Succeeded)
               {
                    await userManager.AddToRoleAsync(user.Id, "Admin");
               }
            }
        }
}
