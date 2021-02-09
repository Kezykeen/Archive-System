using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class StaffRepository: IStaffRepository
    {
        private readonly ApplicationDbContext _db;

        public StaffRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool EmailExists(string email)
        {
            var staff =   _db.Staffs.SingleOrDefault(m => m.Email == email);
            return staff != null;
        }
    }
}