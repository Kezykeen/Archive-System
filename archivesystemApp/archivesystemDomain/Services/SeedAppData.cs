using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using archivesystemDomain.Entities;
using Microsoft.AspNet.Identity.Owin;

namespace archivesystemDomain.Services
{
    public static class SeedAppData
    {
        private static HttpContext Context => HttpContext.Current;
        private static ApplicationDbContext DbContext
            => Context.GetOwinContext().Get<ApplicationDbContext>();
        public static void EnsurePopulated()
        {

            if (!DbContext.Faculties.Any())
            {
                DbContext.Faculties.AddRange(

                    new List<Faculty>()
                    {
                        new Faculty()
                        {
                            Name = "Engineering",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        new Faculty()
                        {
                            Name = "Arts",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                    }
                );
            }

            if (!DbContext.Departments.Any())
            {
                DbContext.Departments.AddRange(

                    new List<Department>()
                    {
                        new Department
                        {
                            Name = "Electronic Engineering",
                            FacultyId = 1,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },

                        new Department
                        {
                            Name = "People and Culture",
                            FacultyId = 2,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                    }
                );
            }

            DbContext.SaveChanges();

        }
       

    }
}