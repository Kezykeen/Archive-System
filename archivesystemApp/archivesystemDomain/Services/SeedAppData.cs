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
       
       
        public static void EnsurePopulated()
        {
            var dbContext = new ApplicationDbContext();

            if (!dbContext.Departments.Any())
            {
                dbContext.Departments.AddRange(

                    new List<Department>()
                    {
                        new Department
                        {
                            Name = "Electronic Engineering",
                            Faculty =  new Faculty()
                            {
                                Name = "Engineering",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            },
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },

                        new Department
                        {
                            Name = "People and Culture",
                            Faculty =  new Faculty()
                            {
                                Name = "Arts",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            },
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                    }
                );
            }

            dbContext.SaveChanges();

        }
       

    }
}