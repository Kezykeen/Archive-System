using System;
using System.Collections;
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
    public class DepartmentService : IDepartmentService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IFacultyRepository _facultyRepository;
        private readonly IFolderRepo _folderRepo;
        private readonly IDeptRepository _deptRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccessLevelRepository _accessLevelRepository;
        #endregion

        #region Constructors

        public DepartmentService(IUnitOfWork unitOfWork, IFacultyRepository facultyRepository, IFolderRepo folderRepo,
            IDeptRepository deptRepository, IUserRepository userRepository, IAccessLevelRepository accessLevelRepository)
        {
            _unitOfWork = unitOfWork;
            _facultyRepository = facultyRepository;
            _folderRepo = folderRepo;
            _deptRepository = deptRepository;
            _userRepository = userRepository;
            _accessLevelRepository = accessLevelRepository;
        }

        #endregion

        #region Public Methods

        public IEnumerable<Department> GetAllDepartmentToList()
        {
            return _deptRepository.GetAllToList();
        }

        public IEnumerable<Faculty> GetAllFaculties()
        {
            return _facultyRepository.GetAll();
        }

        public Department GetDepartmentForPartialView(int id)
        {
            var department = _deptRepository.Get(id);

            return department;
        }

        public Department GetDepartmentById(int id)
        {
            return _deptRepository.Get(id);
        }

        public (ServiceResult, string message) SaveDepartment(Department department)
        {
            var folderExists = DoesDepartmentFolderAlreadyExist(department);
            var result = folderExists ? SaveOnlyDepartment(department) : TrySaveDepartmentWithFolder(department);

            return result;
        }

        public (ServiceResult, string message) UpdateDepartment(Department department)
        {
            var status = DoesDepartmentNameExist(department.Name, department.Id);
            if (status)
                return (ServiceResult.Failure, "Name already exist");
            try
            {
                _deptRepository.Update(department);

                return (ServiceResult.Succeeded, "");
            }
            catch (Exception e)
            {
                return (ServiceResult.Failure, e.Message);
            }
        }

        public void UpdateDepartmentFolder(Folder folder)
        {
            _folderRepo.UpdateDepartmentalFolder(folder);
        }

        public async Task<ServiceResult> DeleteDepartment(int id)
        {
            var departmentContainsUsers = DoesDepartmentContainUsers(id);
            if (departmentContainsUsers)
                return ServiceResult.Prohibited;
            var departmentFolder = _folderRepo.Find(x => x.DepartmentId == id).FirstOrDefault();
            if (departmentFolder != null)
                departmentFolder.DepartmentId = null;

            await Delete(id);

            return ServiceResult.Succeeded;
        }

        public DepartmentUsersViewModel GetAllUsersInDepartment(int id)
        {
            var viewModel = new DepartmentUsersViewModel
            {
                Users = _userRepository.Find(u => u.DepartmentId == id),
                Department = GetDepartmentById(id)
            };

            return viewModel;
        }

        public async Task SaveChanges()
        {
            await _unitOfWork.SaveAsync();
        }

        public bool DoesDepartmentNameExist(string name, int id)
        {
            var departments = GetAllDepartmentToList();

            // Check if the entry name exists & change is from a different entry and return error message from viewModel
            bool status = departments.Any(x => x.Name == name && x.Id != id);
            return status;
        }

        public IEnumerable GetDepartments(int id, string searchTerm = null)
        {
            var departments = _deptRepository.Find(d => d.Name.Contains(searchTerm) && d.Id != id).Select(x => new
            {
                id = x.Id,
                text = x.Name
            });
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                departments = _deptRepository.Find(d => d.Id != id).Select(x => new
                {
                    id = x.Id,
                    text = x.Name
                });
            }

            return departments;
        }

        #endregion

            #region Private Methods

            private bool DoesDepartmentContainUsers(int departmentId)
        {
            var appUsers = _userRepository.Find(x => x.DepartmentId == departmentId);
            return appUsers.Any();
        }

        private bool DoesDepartmentFolderAlreadyExist(Department department)
        {
            var faculty = _facultyRepository.Get(department.FacultyId);
            if (faculty == null)
                throw new ArgumentNullException();

            var facultyfolder = _folderRepo.GetFacultyFolder(faculty.Name);
            if (facultyfolder == null)
                throw new ArgumentNullException();

            if (facultyfolder.Subfolders == null)
                return false;

            var deptFolderNames = facultyfolder.Subfolders.Select(x => x.Name);
            return deptFolderNames.Contains(department.Name);

        }

        private
        (ServiceResult, string message) TrySaveDepartmentWithFolder(Department department)
        {
            try
            {
                var faculty = _facultyRepository.Get(department.FacultyId);
                var facultyFolder = _folderRepo.GetFacultyFolder(faculty.Name);
                var folder = new Folder
                {
                    Name = department.Name,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    AccessLevelId = _accessLevelRepository.GetBaseLevel().Id,
                    ParentId = facultyFolder.Id,
                    IsRestricted = true,
                    Department = department
                };
                //The folder object contains the department to be created
                //adding the department folder to the context also adds the 
                //new department to the context.
                _folderRepo.Add(folder);
                _unitOfWork.Save();
                return (ServiceResult.Succeeded, "");
            }
            catch (Exception e)
            {
                return (ServiceResult.Failure, e.Message);
            }
        }

        private (ServiceResult, string message) SaveOnlyDepartment(Department department)
        {
            var status = DoesDepartmentNameExist(department.Name, department.Id);
            if (status)
                return (ServiceResult.Failure, "Name already exist");
            try
            {
                _deptRepository.Add(department);
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
            var department = GetDepartmentById(id);
            _deptRepository.Remove(department);
            await _unitOfWork.SaveAsync();
        }
        #endregion
    }
}