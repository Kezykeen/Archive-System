using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Services
{
    public class FacultyService : IFacultyService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IFacultyRepository _facultyRepository;
        private readonly IFolderRepo _folderRepo;
        private readonly IDeptRepository _deptRepository;
        private readonly IAccessLevelRepository _accessLevelRepository;

        #endregion

        #region Constructors

        public FacultyService(IUnitOfWork unitOfWork, IFacultyRepository facultyRepository, IFolderRepo folderRepo, IDeptRepository deptRepository, IAccessLevelRepository accessLevelRepository)
        {
            _unitOfWork = unitOfWork;
            _facultyRepository = facultyRepository;
            _folderRepo = folderRepo;
            _deptRepository = deptRepository;
            _accessLevelRepository = accessLevelRepository;
        }
        #endregion

        #region Public methods

        public IEnumerable<Faculty> GetAllFacultiesToList()
        {
            return _facultyRepository.GetAllToList();
        }

        public Faculty GetFacultyForPartialView(int id)
        {
            var faculty = _facultyRepository.Get(id);

            return faculty;
        }

        public Faculty GetFacultyById(int id)
        {
            return _facultyRepository.Get(id);
        }

        public (ServiceResult, string message) SaveFaculty(Faculty faculty)
        {
            var facultyFolderExist = DoesFacultyFolderAlreadyExist(faculty);
            var result = facultyFolderExist ? SaveOnlyFaculty(faculty) : TrySaveFacultyWithFolder(faculty);

            return result;
        }

        public (ServiceResult, string message) UpdateFaculty(Faculty faculty)
        {
            var status = DoesFacultyNameExist(faculty.Name, faculty.Id);
            if (status)
                return (ServiceResult.Failure, "Name already exist");
            try
            {
                _facultyRepository.Update(faculty);

                return (ServiceResult.Succeeded, "");
            }
            catch (Exception e)
            {
                return (ServiceResult.Failure, e.Message);
            }
        }

        public void UpdateFacultyFolder(Folder folder)
        {
            _folderRepo.UpdateFacultyFolder(folder);
        }

        public async Task<ServiceResult> DeleteFaculty(int id)
        {
            var facultyContainsDepartments = DoesFacultyContainDepartments(id);
            if (facultyContainsDepartments)
                return ServiceResult.Prohibited;
            var facultyFolder = _folderRepo.Find(x => x.FacultyId == id).FirstOrDefault();
            if (facultyFolder != null)
                facultyFolder.FacultyId = null;

            await Delete(id);

            return ServiceResult.Succeeded;
        }

        public FacultyDepartmentsViewModel GetAllDepartmentsInFaculty(int id)
        {
            var viewModel = new FacultyDepartmentsViewModel
            {
                Departments = _deptRepository.Find(f => f.FacultyId == id),
                Faculty = GetFacultyById(id)
            };

            return viewModel;
        }

        public async Task SaveChanges()
        {
            await _unitOfWork.SaveAsync();
        }

        public bool DoesFacultyNameExist(string name, int id)
        {
            var faculties = GetAllFacultiesToList();

            // Check if the entry name exists & change is from a different entry and return error message from viewModel
            bool status = faculties.Any(x => x.Name == name && x.Id != id);
            return status;
        }
        #endregion

        #region Private methods

        private bool DoesFacultyContainDepartments(int facultyId)
        {
            var depts = _deptRepository.Find(x => x.FacultyId == facultyId);
            return depts.Any();
        }

        private bool DoesFacultyFolderAlreadyExist(Faculty faculty)
        {
            var folders =
                _folderRepo.FindWithNavProps(x => x.Name == GlobalConstants.RootFolderName,
                    x => x.Subfolders);
            if (folders == null)
                throw new ArgumentNullException();

            var rootFolder = folders.SingleOrDefault();

            if (rootFolder == null)
                throw new ArgumentNullException();

            if (rootFolder.Subfolders == null)
                return false;

            return rootFolder.Subfolders.Select(x => x.Name).Contains(faculty.Name);
        }

        private (ServiceResult, string message) TrySaveFacultyWithFolder(Faculty faculty)
        {
            try
            {
                var rootFolder = _folderRepo.Find(x => x.Name == GlobalConstants.RootFolderName)
                    .Single();
                var folder = new Folder
                {
                    Name = faculty.Name,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    AccessLevelId = _accessLevelRepository.GetBaseLevel().Id,
                    ParentId = rootFolder.Id,
                    IsRestricted = true,
                    Faculty = faculty
                };
                //The folder object contains the faculty to be created
                //adding the faculty folder to the context also adds the 
                //new faculty to the context.
                _folderRepo.Add(folder);
                _unitOfWork.Save();
                return (ServiceResult.Succeeded, "");
            }
            catch (Exception e)
            {
                return (ServiceResult.Failure, e.Message);
            }
        }

        private (ServiceResult, string message) SaveOnlyFaculty(Faculty faculty)
        {
            var status = DoesFacultyNameExist(faculty.Name, faculty.Id);
            if (status)
                return (ServiceResult.Failure, "Name already exist");
            try
            {
                _facultyRepository.Add(faculty);
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
                return (ServiceResult.Failure, e.Message);
            }

            return (ServiceResult.Succeeded, "");
        }

        private async Task Delete(int id)
        {
            var faculty = GetFacultyById(id);
            _facultyRepository.Remove(faculty);
            await _unitOfWork.SaveAsync();
        }
        #endregion
    }
}
