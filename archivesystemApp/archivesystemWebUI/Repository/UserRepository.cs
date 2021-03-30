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

      

        public AppUser GetUserByMail(string email)
        {
            var employee = _context.AppUsers.SingleOrDefault(m => m.Email == email);
            return employee;
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