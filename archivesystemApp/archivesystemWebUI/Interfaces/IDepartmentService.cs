using System.Collections.Generic;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Interfaces
{
    public interface IDepartmentService
    {
        IEnumerable<Department> GetAllDepartmentToList();
        IEnumerable<Faculty> GetAllFaculties();
        Department GetDepartment(int? id);
        ServiceResult SaveDepartment(Department department);
        ServiceResult UpdateDepartment(Department department);
        void UpdateDepartmentFolder(Folder folder);
        Task<ServiceResult> DeleteDepartment(int id);
        DepartmentUsersViewModel GetAllUsersInDepartment(int id);
        Task SaveChanges();
        bool DepartNameCheck(string name, int id);
    }
}