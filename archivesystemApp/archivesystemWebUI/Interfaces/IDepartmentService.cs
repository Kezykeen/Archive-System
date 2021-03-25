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
        DepartmentViewModel GetDepartment(int? id);
        ServiceResult SaveDepartment(Department department);
        Task<ServiceResult> UpdateDepartment(Department department);
        Department GetDepartmentInDb(int id);
        Task<ServiceResult> DeleteDepartment(int id);
        Department GetDepartmentById(int id);
        IEnumerable<AppUser> GetAllUsersInDepartment(int id);
        Task Delete(int id);
        bool DepartNameCheck(string name, int id);
    }
}