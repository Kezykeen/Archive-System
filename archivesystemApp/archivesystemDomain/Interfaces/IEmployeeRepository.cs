using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        bool EmailExists(string email, int? userId);
        bool NameExists(string name, int? userId);
        bool StaffIdExists(string staffId, int? userId);
        Employee GetEmployeeByMail(string email);
        IEnumerable<Employee> GetEmployeesWithDept();
        Employee GetEmployeeWithDept(int? id, string appId = null);
        void UpdateUserId(string email, string id);
        bool PhoneExists(string phone, int? userId);
    }
}
