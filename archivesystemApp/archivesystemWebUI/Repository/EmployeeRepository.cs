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

        public Employee GetEmployee(string email)
        {
            var employee = _context.Employees.SingleOrDefault(m => m.Email == email);
            return employee;
        }

        public bool EmailExists(string email)
        {
            var employee = GetEmployee(email);
            return employee != null;
        }


        public void UpdateUserId(string email, string id)
        {
            var employee = GetEmployee(email);
            employee.UserId = id;
           
        }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}