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
    public class EmployeeRepository: Repository<Employee> , IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        :base(context)
        {
            _context = context;
        }

        public bool NameExists(string name)
        {
            return GetAll().Any(e => string.Equals(e.Name, name,
                StringComparison.OrdinalIgnoreCase));
        }

        public Employee GetEmployeeByMail(string email)
        {
            var employee = _context.Employees.SingleOrDefault(m => m.Email == email);
            return employee;
        }

        public bool EmailExists(string email)
        {
            return GetAll().Any(e => string.Equals(e.Email, email,
                StringComparison.OrdinalIgnoreCase));
        }


        public bool StaffIdExists(string staffId)
        {
            return GetAll().Any(e => string.Equals(e.StaffId, staffId,
                StringComparison.OrdinalIgnoreCase));
        }


        public void UpdateUserId(string email, string id)
        {
            var employee = GetEmployeeByMail(email);
            employee.UserId = id;
           
        }

        public bool PhoneExists(string phone)
        {
             return GetAll().Any(e => string.Equals(e.Phone, phone,
                StringComparison.OrdinalIgnoreCase));
        }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}