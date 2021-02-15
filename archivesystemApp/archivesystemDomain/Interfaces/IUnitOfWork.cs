using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Interfaces
{
    public interface IUnitOfWork
    {
        IEmployeeRepository EmployeeRepo { get; }
        IDeptRepository DeptRepo { get; }
        IAccessLevelRepository AccessLevelRepo { get; }
        IFacultyRepository FacultyRepo { get; }
        int Save();
        Task<int> SaveAsync();

    }
}