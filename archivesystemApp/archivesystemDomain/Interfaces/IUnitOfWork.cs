using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Interfaces
{
    public interface IUnitOfWork
    {
        IEmployeeRepository EmployeeRepo { get; }
        IAccessLevelRepository AccessLevelRepo { get; }
        IRoleRepository RoleRepo { get; }
        Task SaveAsync();
        void Save();
    }
}