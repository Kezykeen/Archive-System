using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IEmployeeRepository
    {
        bool EmailExists(string email);
        Employee GetEmployee(string email);
        void UpdateUserId(string email, string id);
    }
}
