using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IUserRepository : IRepository<AppUser>
    {
        AppUser GetUserByMail(string email);
        AppUser GetUserByUserId(string id);
        IEnumerable<RoleMemberData> GetUserDataByUserIds(ICollection<string> userIds);
    }
}
