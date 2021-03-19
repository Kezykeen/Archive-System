using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using archivesystemWebUI.Interfaces;
using archivesystemDomain.Entities;
using AutoMapper;

namespace archivesystemWebUI.Services
{
    public enum FacultyServiceResult
    {
        Succeeded, Failure
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
            var faculty = CreateFaculty(model);
            var result= TrySaveFaculty(faculty);
            return result;
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
            return folder;
        }

        private Faculty CreateFaculty(FacultyViewModel model)
        {
            var faculty = Mapper.Map<Faculty>(model);
            faculty.CreatedAt = DateTime.Now;
            faculty.UpdatedAt = DateTime.Now;
            return faculty;
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
        #endregion
    }
}