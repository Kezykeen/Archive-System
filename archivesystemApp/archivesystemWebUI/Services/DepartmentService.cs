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
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Department> GetAllDepartmentToList()
        {
            return _unitOfWork.DeptRepo.GetAllToList();
        }

        public IEnumerable<Faculty> GetAllFaculties()
        {
            return _unitOfWork.FacultyRepo.GetAll();
        }

        public DepartmentViewModel GetDepartmentViewModel(int? id)
        {
            var department = id != null ? _unitOfWork.DeptRepo.Get(id.Value) : new Department();
            var viewModel = Mapper.Map<DepartmentViewModel>(department);
            return viewModel;
        }

        public void AddOrEdit(DepartmentViewModel model)
        {
            var department = Mapper.Map<Department>(model);
            if (model.Id == 0)
            {
                department.CreatedAt = DateTime.Now;
                department.UpdatedAt = DateTime.Now;
                CreateDepartmentAndFolder(department);
            }
            else
            {
                var folderModel = Mapper.Map<Folder>(model);
                folderModel.DepartmentId = model.Id;
                _unitOfWork.FolderRepo.UpdateDepartmentalFolder(folderModel);

                var getDepartment = _unitOfWork.DeptRepo.Get(model.Id);
                Mapper.Map(model, getDepartment);
                getDepartment.UpdatedAt = DateTime.Now;
                _unitOfWork.DeptRepo.Update(getDepartment);
            }

            _unitOfWork.Save();
        }

        public Department GetDepartmentById(int id)
        {
            return _unitOfWork.DeptRepo.Get(id);
        }

        public async Task Delete(int id)
        {
            Department department = GetDepartmentById(id);
            _unitOfWork.DeptRepo.Remove(department);
            await _unitOfWork.SaveAsync();
        }

        public void CreateDepartmentAndFolder(Department department)
        {
            var faculty = _unitOfWork.FacultyRepo.Get(department.FacultyId);
            var facultyFolder = _unitOfWork.FolderRepo.GetFacultyFolder(faculty.Name);
            var folder = new Folder
            {
                Name = department.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AccessLevelId = _unitOfWork.AccessLevelRepo.GetBaseLevel().Id,
                ParentId = facultyFolder.Id,
                IsRestricted = true,
                Department = department
            };

            _unitOfWork.FolderRepo.Add(folder);
        }

        public bool DepartNameCheck(string name, int id)
        {
            var departments = _unitOfWork.DeptRepo.GetAllToList();

            // Check if the entry name exists & change is from a different entry and return error message from viewModel
            bool status = departments.Any(x => x.Name == name && x.Id != id);
            return status;
        }
    }
}