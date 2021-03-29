using System.Collections.Generic;
using archivesystemDomain.Entities;
using System.Threading.Tasks;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Interfaces
{
    public interface IFacultyService
    {
        ServiceResult SaveFaculty(Faculty faculty);
        ServiceResult UpdateFaculty(Faculty model);
        void UpdateFacultyFolder(Folder folder);
        Task<ServiceResult> DeleteFaculty(int id);
        IEnumerable<Faculty> GetAllFacultiesToList();
        Faculty GetFacultyForPartialView(int id);
        Faculty GetFacultyById(int id);
        FacultyDepartmentsViewModel GetAllDepartmentsInFaculty(int id);
        int GetAllDepartmentsInFacultyCount(int id);
        Task SaveChanges();
        bool FacultyNameCheck(string name, int id);
    }
}