using System.Collections.Generic;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Services
{
    public interface IFacultyService
    {
        IEnumerable<FacultyViewModel> GetFacultyData();
        FacultyViewModel GetFacultyViewModel(int? id);
        Task AddOrEdit(FacultyViewModel model);
        Faculty GetFacultyById(int id);
        Task Delete(int id);
        void CreateFacultyAndFolder(Faculty faculty);
        bool FacultyNameCheck(string name, int id);
    }
}