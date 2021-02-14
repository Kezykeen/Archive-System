using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        bool EmailExists(string email);
        bool NameExists(string name);
        bool StaffIdExists(string staffId);
        Employee GetEmployeeByMail(string email);

        void UpdateUserId(string email, string id);
    }
}
