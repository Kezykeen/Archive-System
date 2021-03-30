using System.Collections;
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
        Department GetDepartmentForPartialView(int id);
        Department GetDepartmentById(int id);
        (ServiceResult, string message) SaveDepartment(Department department);
        (ServiceResult, string message) UpdateDepartment(Department department);
        void UpdateDepartmentFolder(Folder folder);
        Task<ServiceResult> DeleteDepartment(int id);
        DepartmentUsersViewModel GetAllUsersInDepartment(int id);
        IEnumerable GetDepartments(int id, string searchTerm = null);
        Task SaveChanges();
        bool DoesDepartmentNameExist(string name, int id);
    }
}