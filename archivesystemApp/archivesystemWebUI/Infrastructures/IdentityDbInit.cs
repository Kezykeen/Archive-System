using System;
using System.Collections.Generic;
using System.Data.Entity;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Infrastructures
{
    public class IdentityDbInit
        : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(ApplicationDbContext context)
        {
            // initial configuration will go here

            context.Departments.AddRange(
                new List<Department>()
                {
                    new Department()
                    {
                        Name = "Operations",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Department()
                    {
                        Name = "Welfare",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                }
            );
         
        }
    }
}