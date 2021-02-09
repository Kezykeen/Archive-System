using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IStaffRepository
    {
        bool EmailExists(string email);
        Staff GetStaff(string email);
        void UpdateUserId(string email, string id);
    }
}
