using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Models.RoleViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class UserRepository : Repository<AppUser>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        : base(context)
        {
            _context = context;
        }

        public bool NameExists(string name, int? userId)
        {
            if (userId == null)
                return GetAll().Any(e => string.Equals(e.Name, name,
                StringComparison.OrdinalIgnoreCase));
            return GetAll().Any(e => string.Equals(e.Name, name,
                StringComparison.OrdinalIgnoreCase) && e.Id != userId.Value);

        }

        public AppUser GetUserByMail(string email)
        {
            var employee = _context.AppUsers.SingleOrDefault(m => m.Email == email);
            return employee;
        }

        public IEnumerable<AppUser> GetUsersWithDept()
        {
            return _context.AppUsers.Include(e => e.Department).ToList();
        }

        public AppUser GetUserWithDept(int? id, string appId = null)
        {
            if (!string.IsNullOrWhiteSpace(appId))
            {
                return _context.AppUsers.Include(e => e.Department).SingleOrDefault(e => e.UserId == appId);
            }

            return _context.AppUsers.Include(e => e.Department).SingleOrDefault(e => e.Id == id);
        }

        public bool EmailExists(string email, int? userId)
        {

            if (userId == null)
                return GetAll().Any(e => string.Equals(e.Email, email,
                StringComparison.OrdinalIgnoreCase));
            return GetAll().Any(e => string.Equals(e.Email, email,
                StringComparison.OrdinalIgnoreCase) && e.Id != userId.Value);
        }


        public bool TagIdExists(string tagId, int? userId)
        {

            if (userId == null)
                return GetAll().Any(e => string.Equals(e.TagId, tagId,
                StringComparison.OrdinalIgnoreCase));

            return GetAll().Any(e => string.Equals(e.TagId, tagId,
                StringComparison.OrdinalIgnoreCase) && e.Id != userId.Value);
        }


        public void UpdateUserId(string email, string id)
        {
            var employee = GetUserByMail(email);
            employee.UserId = id;

        }

        public bool PhoneExists(string phone, int? userId)
        {
            if (userId == null)
                return GetAll().Any(e => string.Equals(e.Phone, phone,
                StringComparison.OrdinalIgnoreCase));
            return GetAll().Any(e => string.Equals(e.Phone, phone,
                StringComparison.OrdinalIgnoreCase) && e.Id != userId.Value);
        }

        public AppUser GetUserByUserId(string id)
        {
            return _context.AppUsers.Include(x => x.Department).Single(x => x.UserId == id);
        }

        public IEnumerable<RoleMemberData> GetUserDataByUserIds(ICollection<string> userIds )
        {
            var data=_context.AppUsers.Include(x=> x.Department)
                                       .Where(x => userIds.Contains(x.UserId))
                                       .Select(x=> new RoleMemberData{ Name=x.Name ,Email=x.Email, UserId=x.UserId,Department=x.Department })
                                       ;
            return data;
           
        }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}