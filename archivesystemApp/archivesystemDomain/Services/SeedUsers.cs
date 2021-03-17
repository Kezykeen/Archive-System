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
    
        public static class SeedUsers
        {
            private static readonly string AdminUser = WebConfigurationManager.AppSettings["AdminUser"];
            private static readonly string AdminPassword = WebConfigurationManager.AppSettings["AdminPwd"];
            private static readonly string AdminEmail = WebConfigurationManager.AppSettings["AdminEmail"];
            private static readonly string AdminPhone = WebConfigurationManager.AppSettings["AdminPhone"];

            private static readonly string EmpUser = WebConfigurationManager.AppSettings["EmpUser"];
            private static readonly string EmpPassword = WebConfigurationManager.AppSettings["EmpPwd"];
            private static readonly string EmpEmail = WebConfigurationManager.AppSettings["EmpEmail"];
            private static readonly string EmpPhone = WebConfigurationManager.AppSettings["EmpPhone"];

            private static readonly string MangrUser = WebConfigurationManager.AppSettings["MangrUser"];
            private static readonly string MangrPassword = WebConfigurationManager.AppSettings["MangrPwd"];
            private static readonly string MangrEmail = WebConfigurationManager.AppSettings["MangrEmail"];
            private static readonly string MangrPhone = WebConfigurationManager.AppSettings["MangrPhone"];

            private static readonly string NonAcademicUser = WebConfigurationManager.AppSettings["NonAcademicUser"];
            private static readonly string NonAcademicPassword = WebConfigurationManager.AppSettings["NonAcademicPwd"];
            private static readonly string NonAcademicEmail = WebConfigurationManager.AppSettings["NonAcademicEmail"];
            private static readonly string NonAcademicPhone = WebConfigurationManager.AppSettings["NonAcademicPhone"];



        public static async void EnsurePopulated()
            {
              

               var dbContext =  new ApplicationDbContext();
               var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));
               var admin = await userManager.FindByNameAsync(AdminUser);
               var employee = await userManager.FindByNameAsync(EmpUser);
               var manager = await userManager.FindByNameAsync(MangrUser);
               var nonAcademicStaff = await userManager.FindByNameAsync(NonAcademicUser);

               if (admin != null) return;
               admin = new ApplicationUser
                    {UserName = AdminUser, Email = AdminEmail, PhoneNumber = AdminPhone, EmailConfirmed = true };
               var createAdmin = await userManager.CreateAsync(admin, AdminPassword);

               if (createAdmin.Succeeded)
               {
                    await userManager.AddToRoleAsync(admin.Id, "Admin");
               }


               if (employee != null) return;
               employee = new ApplicationUser
                   { UserName = EmpUser, Email = EmpEmail, PhoneNumber = EmpPhone, EmailConfirmed = true };
               var createEmployee = await userManager.CreateAsync(employee, EmpPassword);

               if (manager != null) return;
               manager = new ApplicationUser
                   { UserName = MangrUser, Email = MangrEmail, PhoneNumber = MangrPhone, EmailConfirmed = true };
               var createManager = await userManager.CreateAsync(manager, MangrPassword);

                if ( nonAcademicStaff != null) return;
                nonAcademicStaff = new ApplicationUser
                { UserName = NonAcademicUser, Email = NonAcademicEmail, PhoneNumber = NonAcademicPhone, EmailConfirmed = true };
                var createNonAcademicStaff = await userManager.CreateAsync(nonAcademicStaff, NonAcademicPassword);

                if (createEmployee.Succeeded)
                {
                    await userManager.AddToRoleAsync(employee.Id, "Employee");

                    dbContext.AppUsers.Add(
                        new AppUser
                        {
                            Name = employee.UserName,
                            DepartmentId = 1,
                            Completed = true,
                            Gender = Gender.Male,
                            TagId = "StaffId-01",
                            Email = employee.Email,
                            UserId = employee.Id,
                            Phone = employee.PhoneNumber,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        }
                    );
                    dbContext.SaveChanges();
                }


               if (createManager.Succeeded)
               {
                   await userManager.AddToRoleAsync(manager.Id, "Manager");

                   dbContext.AppUsers.Add(
                       new AppUser
                       {
                           Name = manager.UserName,
                           DepartmentId = 2,
                           Completed = true,
                           Gender = Gender.Female,
                           TagId = "StaffId-02",
                           Email = manager.Email,
                           UserId = manager.Id,
                           Phone = manager.PhoneNumber,
                           CreatedAt = DateTime.Now,
                           UpdatedAt = DateTime.Now
                       }
                   );
                   dbContext.SaveChanges();
               }

               if (createNonAcademicStaff.Succeeded)
               {
                   await userManager.AddToRoleAsync(nonAcademicStaff.Id, "Manager");

                   dbContext.AppUsers.Add(
                       new AppUser
                       {
                           Name = nonAcademicStaff.UserName,
                           DepartmentId = 3,
                           Completed = true,
                           Gender = Gender.Female,
                           TagId = "StaffId-03",
                           Email = nonAcademicStaff.Email,
                           UserId = nonAcademicStaff.Id,
                           Phone = nonAcademicStaff.PhoneNumber,
                           CreatedAt = DateTime.Now,
                           UpdatedAt = DateTime.Now
                       }
                   );
                   dbContext.SaveChanges();
               }
            }
        }
}
