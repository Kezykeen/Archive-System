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

            private static readonly string FacultyUser = WebConfigurationManager.AppSettings["FacultyUser"];
            private static readonly string FacultyPassword = WebConfigurationManager.AppSettings["FacultyPwd"];
            private static readonly string FacultyEmail = WebConfigurationManager.AppSettings["FacultyEmail"];
            private static readonly string FacultyPhone = WebConfigurationManager.AppSettings["FacultyPhone"];

            private static readonly string StudentUser = WebConfigurationManager.AppSettings["StudentUser"];
            private static readonly string StudentPassword = WebConfigurationManager.AppSettings["StudentPwd"];
            private static readonly string StudentEmail = WebConfigurationManager.AppSettings["StudentEmail"];
            private static readonly string StudentPhone = WebConfigurationManager.AppSettings["StudentPhone"];

            private static readonly string NonAcademicUser = WebConfigurationManager.AppSettings["NonAcademicUser"];
            private static readonly string NonAcademicPassword = WebConfigurationManager.AppSettings["NonAcademicPwd"];
            private static readonly string NonAcademicEmail = WebConfigurationManager.AppSettings["NonAcademicEmail"];
            private static readonly string NonAcademicPhone = WebConfigurationManager.AppSettings["NonAcademicPhone"];



        public static async void EnsurePopulated()
            {
              

               var dbContext =  new ApplicationDbContext();
               var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));
               var admin = await userManager.FindByNameAsync(AdminUser);
               var facultyOfficer = await userManager.FindByNameAsync(FacultyUser);
               var manager = await userManager.FindByNameAsync(StudentUser);
               var nonAcademicStaff = await userManager.FindByNameAsync(NonAcademicUser);

               if (admin != null ) return;
               admin = new ApplicationUser
                    {UserName = AdminUser, Email = AdminEmail, PhoneNumber = AdminPhone, EmailConfirmed = true };
               var createAdmin = await userManager.CreateAsync(admin, AdminPassword);

               if (createAdmin.Succeeded)
               {
                    await userManager.AddToRoleAsync(admin.Id, "Admin");
               }


               if (facultyOfficer != null) return;
               facultyOfficer = new ApplicationUser
                   { UserName = FacultyUser, Email = FacultyEmail, PhoneNumber = FacultyPhone, EmailConfirmed = true };
               var createEmployee = await userManager.CreateAsync(facultyOfficer, FacultyPassword);

               if (manager != null) return;
               manager = new ApplicationUser
                   { UserName = StudentUser, Email = StudentEmail, PhoneNumber = StudentPhone, EmailConfirmed = true };
               var createManager = await userManager.CreateAsync(manager, StudentPassword);

                if ( nonAcademicStaff != null) return;
                nonAcademicStaff = new ApplicationUser
                { UserName = NonAcademicUser, Email = NonAcademicEmail, PhoneNumber = NonAcademicPhone, EmailConfirmed = true };
                var createNonAcademicStaff = await userManager.CreateAsync(nonAcademicStaff, NonAcademicPassword);

                if (createEmployee.Succeeded)
                {
                    await userManager.AddToRoleAsync(facultyOfficer.Id, "Staff");
                    await userManager.AddToRoleAsync(facultyOfficer.Id, "FacultyOfficer");

                    dbContext.AppUsers.Add(
                        new AppUser
                        {
                            Name = facultyOfficer.UserName,
                            DepartmentId = 1,
                            Completed = true,
                            Gender = Gender.Male,
                            Designation = Designation.Staff,
                            TagId = "StaffId-01",
                            Email = facultyOfficer.Email,
                            UserId = facultyOfficer.Id,
                            Phone = facultyOfficer.PhoneNumber,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        }
                    );
                    dbContext.SaveChanges();
                }


               if (createManager.Succeeded)
               {
                   await userManager.AddToRoleAsync(manager.Id, "Student");

                   dbContext.AppUsers.Add(
                       new AppUser
                       {
                           Name = manager.UserName,
                           DepartmentId = 2,
                           Completed = true,
                           Gender = Gender.Female,
                           Designation = Designation.Student,
                           TagId = "2021/198345",
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
                   await userManager.AddToRoleAsync(nonAcademicStaff.Id, "Staff");

                   dbContext.AppUsers.Add(
                       new AppUser
                       {
                           Name = nonAcademicStaff.UserName,
                           DepartmentId = 3,
                           Completed = true,
                           Gender = Gender.Female,
                           Designation = Designation.Staff,
                           TagId = "StaffId-02",
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
