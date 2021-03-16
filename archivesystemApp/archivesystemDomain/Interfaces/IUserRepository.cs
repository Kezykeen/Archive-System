using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IUserRepository : IRepository<AppUser>
    {
        bool EmailExists(string email, int? userId);
        bool NameExists(string name, int? userId);
        bool TagIdExists(string tagId, int? userId);
        AppUser GetUserByMail(string email);
        IEnumerable<AppUser> GetUsersWithDept();
        AppUser GetUserWithDept(int? id, string appId = null);
        void UpdateUserId(string email, string id);
        bool PhoneExists(string phone, int? userId);
        AppUser GetUserByUserId(string id);
        IEnumerable<RoleMemberData> GetUserDataByUserIds(ICollection<string> userIds);
    }
}
