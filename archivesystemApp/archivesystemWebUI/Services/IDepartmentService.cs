using System.Collections.Generic;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Services
{
    public interface IDepartmentService
    {
        IEnumerable<Department> GetAllDepartmentToList();
        IEnumerable<Faculty> GetAllFaculties();
        DepartmentViewModel GetDepartmentViewModel(int? id);
        void AddOrEdit(DepartmentViewModel model);
        Department GetDepartmentById(int id);
        Task Delete(int id);
        void CreateDepartmentAndFolder(Department department);
        bool DepartNameCheck(string name, int id);
    }
}