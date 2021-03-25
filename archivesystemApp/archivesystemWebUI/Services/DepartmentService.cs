﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Services
{
    public class DepartmentService : IDepartmentService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructors

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion
        
        #region Public Methods

        public IEnumerable<Department> GetAllDepartmentToList()
        {
            return _unitOfWork.DeptRepo.GetAllToList();
        }

        public IEnumerable<Faculty> GetAllFaculties()
        {
            return _unitOfWork.FacultyRepo.GetAll();
        }

        public DepartmentViewModel GetDepartment(int? id)
        {
            var department = id != null ? _unitOfWork.DeptRepo.Get(id.Value) : new Department();
            var viewModel = Mapper.Map<DepartmentViewModel>(department);
            return viewModel;
        }

        public ServiceResult SaveDepartment(Department model)
        {
            var folderExists = DoesDepartmentFolderAlreadyExist(model);
            var department = CreateDepartment(model);
            var result = folderExists ? SaveOnlyDepartment(department) : TrySaveDepartmentWithFolder(department);

            return result;
        }

        public async Task<ServiceResult> UpdateDepartment(Department department)
        {
            try
            {
                department.UpdatedAt = DateTime.Now;
                _unitOfWork.DeptRepo.Update(department);

                var folder = Mapper.Map<Folder>(department);
                _unitOfWork.FolderRepo.UpdateFacultyFolder(folder);

                await _unitOfWork.SaveAsync();

                return ServiceResult.Succeeded;
            }
            catch
            {
                return ServiceResult.Failure;
            }
        }

        public Department GetDepartmentInDb(int id)
        {
            return _unitOfWork.DeptRepo.Get(id);
        }

        public async Task<ServiceResult> DeleteDepartment(int id)
        {
            var departmentContainsUsers = DoesDepartmentContainUsers(id);
            if (departmentContainsUsers)
                return ServiceResult.Prohibited;
            var departmentFolder = _unitOfWork.FolderRepo.Find(x => x.DepartmentId == id).FirstOrDefault();
            if (departmentFolder != null)
                departmentFolder.DepartmentId = null;

            Department department = _unitOfWork.DeptRepo.Get(id);
            _unitOfWork.DeptRepo.Remove(department);
            await _unitOfWork.SaveAsync();

            return ServiceResult.Succeeded;
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

        public bool DepartNameCheck(string name, int id)
        {
            var departments = _unitOfWork.DeptRepo.GetAllToList();

            // Check if the entry name exists & change is from a different entry and return error message from viewModel
            bool status = departments.Any(x => x.Name == name && x.Id != id);
            return status;
        }

        public IEnumerable<AppUser> GetAllUsersInDepartment(int id)
        {
            var users = _unitOfWork.UserRepo.Find(u => u.DepartmentId == id).ToList();
            return users;
        }
        #endregion

        #region Private Methods

        private bool DoesDepartmentContainUsers(int departmentId)
        {
            var appUsers = _unitOfWork.UserRepo.Find(x => x.DepartmentId == departmentId);
            if (appUsers.Any())
                return true;
            return false;
        }

        private bool DoesDepartmentFolderAlreadyExist(Department department)
        {
            var faculty = _unitOfWork.FacultyRepo.Get(department.FacultyId);
            if (faculty == null)
                throw new ArgumentNullException();

            var facultyfolder = _unitOfWork.FolderRepo.GetFacultyFolder(faculty.Name);
            if (facultyfolder == null)
                throw new ArgumentNullException();

            if (facultyfolder.Subfolders == null)
                return false;

            var deptFolderNames = facultyfolder.Subfolders.Select(x => x.Name);
            return deptFolderNames.Contains(department.Name);

        }

        private Department CreateDepartment(Department department)
        {
            department.CreatedAt = DateTime.Now;
            department.UpdatedAt = DateTime.Now;
            return department;
        }

        private Folder CreateDepartmentAndFolder(Department department)
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

            return folder;
        }

        private ServiceResult TrySaveDepartmentWithFolder(Department department)
        {
            try
            {
                var departmentFolder = CreateDepartmentAndFolder(department);
                //The departmentFolder object contains the department to be created
                //adding the department folder to the context also adds the 
                //new department to the context.
                _unitOfWork.FolderRepo.Add(departmentFolder);
                _unitOfWork.Save();
                return ServiceResult.Succeeded;
            }
            catch
            {
                return ServiceResult.Failure;
            }
        }

        private ServiceResult SaveOnlyDepartment(Department department)
        {
            _unitOfWork.DeptRepo.Add(department);
            _unitOfWork.Save();

            return ServiceResult.Succeeded;
        }
        #endregion
    }
}