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
    }
}