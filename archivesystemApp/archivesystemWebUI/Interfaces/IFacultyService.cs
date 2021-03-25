using System.Collections.Generic;
using archivesystemDomain.Entities;
using System.Threading.Tasks;
using archivesystemDomain.Services;

namespace archivesystemWebUI.Interfaces
{
    public interface IFacultyService
    {
        ServiceResult SaveFaculty(Faculty faculty);
        Task<ServiceResult> UpdateFaculty(Faculty model);
        Faculty GetFacultyInDb(int id);
        Task<ServiceResult> DeleteFaculty(int id);
        IEnumerable<Faculty> GetFacultyData();
        Faculty GetFaculty(int? id);
        Faculty GetFacultyById(int id);
        IEnumerable<Department> GetAllDepartmentsInFaculty(int id);
        bool FacultyNameCheck(string name, int id);
    }
}