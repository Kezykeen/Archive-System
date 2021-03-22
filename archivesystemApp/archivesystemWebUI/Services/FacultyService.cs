using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Services
{
    public class FacultyService : IFacultyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacultyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<FacultyViewModel> GetFacultyData()
        {
            var faculty = _unitOfWork.FacultyRepo.GetAllToList();
            var map = Mapper.Map<IEnumerable<FacultyViewModel>>(faculty);

            return map;
        }

        public FacultyViewModel GetFacultyViewModel(int? id)
        {
            var faculty = id != null ? _unitOfWork.FacultyRepo.Get(id.Value) : new Faculty();
            var model = Mapper.Map<FacultyViewModel>(faculty);
            return model;
        }

        public async Task AddOrEdit(FacultyViewModel model)
        {
            var faculty = Mapper.Map<Faculty>(model);
            if (model.Id == 0)
            {
                faculty.CreatedAt = DateTime.Now;
                faculty.UpdatedAt = DateTime.Now;
                CreateFacultyAndFolder(faculty);
            }
            else
            {
                var getFaculty = _unitOfWork.FacultyRepo.Get(model.Id);
                Mapper.Map(model, getFaculty);
                getFaculty.UpdatedAt = DateTime.Now;

                _unitOfWork.FacultyRepo.Update(getFaculty);
                var folder = Mapper.Map<Folder>(model);
                _unitOfWork.FolderRepo.UpdateFacultyFolder(folder);
            }

            await _unitOfWork.SaveAsync();
        }

        public Faculty GetFacultyById(int id)
        {
            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
            return faculty;
        }

        public async Task Delete(int id)
        {
            Faculty faculty = GetFacultyById(id);
            _unitOfWork.FacultyRepo.Remove(faculty);
            await _unitOfWork.SaveAsync();
        }

        public void CreateFacultyAndFolder(Faculty faculty)
        {
            var rootFolder = _unitOfWork.FolderRepo.GetRootFolder();

namespace archivesystemWebUI.Services
{
    public enum FacultyServiceResult
    {
        Succeeded, Failure, Prohibited
    }
    public class FacultyService:IFacultyService
    {
        private IUnitOfWork _unitOfWork;
        public FacultyService(IUnitOfWork repo)
        {
            _unitOfWork = repo;
        } 
        public FacultyServiceResult SaveFaculty (FacultyViewModel model)
        {
            var facultyAlreadyExist = DoesFacultyAlreadyExist(model);
            if (facultyAlreadyExist) return FacultyServiceResult.Failure;

            var facultyFolderExist=DoesFacultyFolderAlreadyExist(model);
            var faculty = CreateFaculty(model);
            FacultyServiceResult result;
            if (!facultyFolderExist)
                result = TrySaveFaculty(faculty);
            else
                result = TrySaveFaculty(faculty, onlyCreateFaculty:true);
            
            return result;
        }
        
        public async Task<FacultyServiceResult> DeleteFaculty(int id)
        {
            var facultyContainsDepartments = DoesFacultyStillContainDepartments(id);
            if (facultyContainsDepartments)
                return FacultyServiceResult.Prohibited;
            var facultyFolder = _unitOfWork.FolderRepo.Find(x => x.FacultyId == id).FirstOrDefault();
            if (facultyFolder != null)
                facultyFolder.FacultyId = null;

            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
            _unitOfWork.FacultyRepo.Remove(faculty);
            await _unitOfWork.SaveAsync();

            return FacultyServiceResult.Succeeded;
        }

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
            _unitOfWork.FolderRepo.Add(folder);
        }

        public bool FacultyNameCheck(string name, int id)
        {
            var faculties = _unitOfWork.FacultyRepo.GetAllToList();

            // Check if the entry name exists & change is from a different entry and return error message from viewModel
            bool status = faculties.Any(x => x.Name == name && x.Id != id);
            return status;
        }
            return folder;
        }

        private bool DoesFacultyStillContainDepartments(int facultyId)
        {
            var depts= _unitOfWork.DeptRepo.Find(x => x.FacultyId == facultyId);
            if (depts.Count() > 0)
                return true;
            return false;
        }
        private Faculty CreateFaculty(FacultyViewModel model)
        {
            var faculty = Mapper.Map<Faculty>(model);
            faculty.CreatedAt = DateTime.Now;
            faculty.UpdatedAt = DateTime.Now;
            return faculty;
        }
        private bool DoesFacultyFolderAlreadyExist(FacultyViewModel model)
        {
            var rootfolder=_unitOfWork.FolderRepo.FindWithNavProps(x => x.Name == GlobalConstants.RootFolderName, x => x.Subfolders);
            var facultyFolderNames = rootfolder.FirstOrDefault().Subfolders.Select(x => x.Name);

            if (facultyFolderNames.Contains(model.Name))
                return true;
            return false;
        }
        private bool DoesFacultyAlreadyExist(FacultyViewModel model)
        {
            var faculty =_unitOfWork.FacultyRepo.Find(x => x.Name == model.Name).SingleOrDefault ();
            if(faculty==null) return false;

            return true;
        }
        private FacultyServiceResult TrySaveFaculty(Faculty faculty)
        {
            try
            {
                var facultyFolder = CreateCorrespondingFolder(faculty);
                //The facultyFolder object contains the faculty to be created
                //adding the faculty folder to the context also adds the 
                //new faculty to the context.
                _unitOfWork.FolderRepo.Add(facultyFolder);
                _unitOfWork.Save();
                return FacultyServiceResult.Succeeded;
            }
            catch { return FacultyServiceResult.Failure; }
            
        }

        private FacultyServiceResult TrySaveFaculty(Faculty faculty, bool onlyCreateFaculty=true)
        {
            _unitOfWork.FacultyRepo.Add(faculty);
            var j = faculty.Id;
            _unitOfWork.Save();

            return FacultyServiceResult.Succeeded;
        }
        #endregion
    }
}