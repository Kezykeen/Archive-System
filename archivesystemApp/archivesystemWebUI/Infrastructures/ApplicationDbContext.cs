using System.Data.Entity;
using archivesystemDomain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace archivesystemWebUI.Infrastructures
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

       
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}