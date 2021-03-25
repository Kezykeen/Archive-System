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

        #endregion

        #region Constructors

        public FacultyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Public methods

        public IEnumerable<Faculty> GetFacultyData()
        {
            return _unitOfWork.FacultyRepo.GetAllToList();
        }

        public Faculty GetFaculty(int? id)
        {
            var faculty = id != null ? _unitOfWork.FacultyRepo.Get(id.Value) : new Faculty();
            
            return faculty;
        }

        public ServiceResult SaveFaculty(Faculty model)
        {
            var facultyFolderExist = DoesFacultyFolderAlreadyExist(model);
            var faculty = CreateFaculty(model);
            var result = facultyFolderExist ? SaveOnlyFaculty(faculty) : TrySaveFacultyWithFolder(faculty);

            return result;
        }

        public ServiceResult UpdateFaculty(Faculty faculty)
        {
            try
            {
                _unitOfWork.FacultyRepo.Update(faculty);

                return ServiceResult.Succeeded;
            }
            catch
            {
                return ServiceResult.Failure;
            }
        }

        public void UpdateFacultyFolder(Folder folder)
        {
            _unitOfWork.FolderRepo.UpdateFacultyFolder(folder);
        }

        public async Task<ServiceResult> DeleteFaculty(int id)
        {
            var facultyContainsDepartments = DoesFacultyContainDepartments(id);
            if (facultyContainsDepartments)
                return ServiceResult.Prohibited;
            var facultyFolder = _unitOfWork.FolderRepo.Find(x => x.FacultyId == id).FirstOrDefault();
            if (facultyFolder != null)
                facultyFolder.FacultyId = null;

            await Delete(id);

            return ServiceResult.Succeeded;
        }

        public FacultyDepartmentsViewModel GetAllDepartmentsInFaculty(int id)
        {
            var viewModel = new FacultyDepartmentsViewModel
            {
                Departments = _unitOfWork.DeptRepo.Find(f=>f.FacultyId == id),
                Faculty = GetFaculty(id)
            };

            return viewModel;
        }

        public async Task SaveChanges()
        {
            await _unitOfWork.SaveAsync();
        }

        public bool FacultyNameCheck(string name, int id)
        {
            var faculties = _unitOfWork.FacultyRepo.GetAllToList();

            // Check if the entry name exists & change is from a different entry and return error message from viewModel
            bool status = faculties.Any(x => x.Name == name && x.Id != id);
            return status;
        }
        #endregion

        #region Private methods

        private Folder CreateCorrespondingFolder(Faculty faculty)
        {
            var rootFolder = _unitOfWork.FolderRepo.Find(x => x.Name == GlobalConstants.RootFolderName)
                .Single();
            var folder = new Folder
            {
                Name = faculty.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AccessLevelId = _unitOfWork.AccessLevelRepo.GetBaseLevel().Id,
                ParentId = rootFolder.Id,
                IsRestricted = true,
                Faculty = faculty
            };
            return folder;
        }

        private bool DoesFacultyContainDepartments(int facultyId)
        {
            var depts = _unitOfWork.DeptRepo.Find(x => x.FacultyId == facultyId);
            return depts.Any();
        }

        private Faculty CreateFaculty(Faculty faculty)
        {
            faculty.CreatedAt = DateTime.Now;
            faculty.UpdatedAt = DateTime.Now;
            return faculty;
        }

        private bool DoesFacultyFolderAlreadyExist(Faculty faculty)
        {
            var folders =
                _unitOfWork.FolderRepo.FindWithNavProps(x => x.Name == GlobalConstants.RootFolderName,
                    x => x.Subfolders);
            var rootFolder = folders.SingleOrDefault();

            return rootFolder?.Subfolders != null && rootFolder.Subfolders.Select(x=>x.Name).Contains(faculty.Name);
        }

        private ServiceResult TrySaveFacultyWithFolder(Faculty faculty)
        {
            try
            {
                var facultyFolder = CreateCorrespondingFolder(faculty);
                //The facultyFolder object contains the faculty to be created
                //adding the faculty folder to the context also adds the 
                //new faculty to the context.
                _unitOfWork.FolderRepo.Add(facultyFolder);
                _unitOfWork.Save();
                return ServiceResult.Succeeded;
            }
            catch
            {
                return ServiceResult.Failure;
            }
        }

        private ServiceResult SaveOnlyFaculty(Faculty faculty)
        {
            _unitOfWork.FacultyRepo.Add(faculty);
            _unitOfWork.Save();

            return ServiceResult.Succeeded;
        }

        private async Task Delete(int id)
        {
            Faculty faculty = GetFaculty(id);
            _unitOfWork.FacultyRepo.Remove(faculty);
            await _unitOfWork.SaveAsync();
        }
        #endregion
    }
}
