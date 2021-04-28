using System.Collections.Generic;
using archivesystemDomain.Entities;
using System.Threading.Tasks;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Interfaces
{
    public interface IFacultyService
    {
        (ServiceResult, string message) SaveFaculty(Faculty faculty);
        (ServiceResult, string message) UpdateFaculty(Faculty model);
        void UpdateFacultyFolder(Folder folder);
        Task<ServiceResult> DeleteFaculty(int id);
        IEnumerable<Faculty> GetAllFacultiesToList();
        Faculty GetFacultyForPartialView(int id);
        Faculty GetFacultyById(int id);
        FacultyDepartmentsViewModel GetAllDepartmentsInFaculty(int id);
        Task SaveChanges();
        bool DoesFacultyNameExist(string name, int id);
    }
}