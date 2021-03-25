using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using AutoMapper;

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

        public Faculty GetFacultyById(int id)
        {
            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);

            return faculty;
        }

        public ServiceResult SaveFaculty(Faculty model)
        {
            var facultyFolderExist = DoesFacultyFolderAlreadyExist(model);
            var faculty = CreateFaculty(model);
            var result = facultyFolderExist ? SaveOnlyFaculty(faculty) : TrySaveFacultyWithFolder(faculty);

            return result;
        }

        public async Task<ServiceResult> UpdateFaculty(Faculty faculty)
        {
            try
            {
                faculty.UpdatedAt = DateTime.Now;
                _unitOfWork.FacultyRepo.Update(faculty);

                var folder = Mapper.Map<Folder>(faculty);
                _unitOfWork.FolderRepo.UpdateFacultyFolder(folder);

                await _unitOfWork.SaveAsync();

                return ServiceResult.Succeeded;
            }
            catch
            {
                return ServiceResult.Failure;
            }
        }

        public Faculty GetFacultyInDb(int id)
        {
            return _unitOfWork.FacultyRepo.Get(id);
        }

        public async Task<ServiceResult> DeleteFaculty(int id)
        {
            var facultyContainsDepartments = DoesFacultyContainDepartments(id);
            if (facultyContainsDepartments)
                return ServiceResult.Prohibited;
            var facultyFolder = _unitOfWork.FolderRepo.Find(x => x.FacultyId == id).FirstOrDefault();
            if (facultyFolder != null)
                facultyFolder.FacultyId = null;

            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
            _unitOfWork.FacultyRepo.Remove(faculty);
            await _unitOfWork.SaveAsync();

            return ServiceResult.Succeeded;
        }

        public IEnumerable<Department> GetAllDepartmentsInFaculty(int id)
        {
            var departments = _unitOfWork.DeptRepo.FindWithNavProps(d => d.FacultyId == id, d=>d.Faculty);

            return departments;
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
            if (depts.Any())
                return true;
            return false;
        }

        private Faculty CreateFaculty(Faculty faculty)
        {
            faculty.CreatedAt = DateTime.Now;
            faculty.UpdatedAt = DateTime.Now;
            return faculty;
        }

        private bool DoesFacultyFolderAlreadyExist(Faculty faculty)
        {
            var rootfolder =
                _unitOfWork.FolderRepo.FindWithNavProps(x => x.Name == GlobalConstants.RootFolderName,
                    x => x.Subfolders);
            var facultyFolderNames = rootfolder.FirstOrDefault()?.Subfolders.Select(x => x.Name);

            if (facultyFolderNames != null && facultyFolderNames.Contains(faculty.Name))
                return true;
            return false;
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
        #endregion
    }
}
