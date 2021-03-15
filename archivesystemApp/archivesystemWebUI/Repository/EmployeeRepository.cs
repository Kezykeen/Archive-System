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
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
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

        public Employee GetEmployeeByMail(string email)
        {
            var employee = _context.Employees.SingleOrDefault(m => m.Email == email);
            return employee;
        }

        public IEnumerable<Employee> GetEmployeesWithDept()
        {
            return _context.Employees.Include(e => e.Department).ToList();
        }

        public Employee GetEmployeeWithDept(int? id, string appId = null)
        {
            if (!string.IsNullOrWhiteSpace(appId))
            {
                return _context.Employees.Include(e => e.Department).SingleOrDefault(e => e.UserId == appId);
            }

            return _context.Employees.Include(e => e.Department).SingleOrDefault(e => e.Id == id);
        }

        public bool EmailExists(string email, int? userId)
        {

            if (userId == null)
                return GetAll().Any(e => string.Equals(e.Email, email,
                StringComparison.OrdinalIgnoreCase));
            return GetAll().Any(e => string.Equals(e.Email, email,
                StringComparison.OrdinalIgnoreCase) && e.Id != userId.Value);
        }


        public bool StaffIdExists(string staffId, int? userId)
        {

            if (userId == null)
                return GetAll().Any(e => string.Equals(e.StaffId, staffId,
                StringComparison.OrdinalIgnoreCase));

            return GetAll().Any(e => string.Equals(e.StaffId, staffId,
                StringComparison.OrdinalIgnoreCase) && e.Id != userId.Value);
        }


        public void UpdateUserId(string email, string id)
        {
            var employee = GetEmployeeByMail(email);
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

        public Employee GetEmployeeByUserId(string id)
        {
            return _context.Employees.Include(x => x.Department).Single(x => x.UserId == id);
        }

        public IEnumerable<RoleMemberData> GetUserDataByUserIds(ICollection<string> userIds )
        {
            var data=_context.Employees.Include(x=> x.Department)
                                       .Where(x => userIds.Contains(x.UserId))
                                       .Select(x=> new RoleMemberData{ Name=x.Name ,Email=x.Email, UserId=x.UserId,Department=x.Department })
                                       ;
            return data;
           
        }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}