using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using archivesystemWebUI.Services;
using Moq;
using NUnit.Framework;

namespace archivesystemApp.UnitTests.DepartmentServiceTests
{
    [TestFixture]
    class DepartmentServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IFolderRepo> _folderRepo;
        private Mock<IDeptRepository> _deptRepo;
        private Mock<IFacultyRepository> _facultyRepo;
        private Mock<IUserRepository> _userRepo;
        private Mock<IAccessLevelRepository> _accessLevelRepo;
        private Department _department;
        private List<Department> _departments;
        private List<AppUser> _users;
        private AppUser _user;
        private Folder _folder;
        private List<Folder> _folders;
        private DepartmentService _service;
        private Faculty _faculty;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _folderRepo = new Mock<IFolderRepo>();
            _deptRepo = new Mock<IDeptRepository>();
            _facultyRepo = new Mock<IFacultyRepository>();
            _userRepo = new Mock<IUserRepository>();
            _accessLevelRepo = new Mock<IAccessLevelRepository>();
            _department = new Department {Id = 1, Name = "English", FacultyId = 1};
            _departments = new List<Department>
            {
                _department
            };
            _user = new AppUser
            {
                DepartmentId = 1, Id = 1, Name = "Steve", Email = "user@domain.com"
            };
            _users = new List<AppUser>
            {
                _user
            };
            _folder =  new Folder
            {
                Id = 1, Name = GlobalConstants.RootFolderName, Subfolders = new List<Folder>
                {
                    new Folder {Name = _department.Name, ParentId = 1}
                }
            };
            _folders = new List<Folder>
            {
                _folder
            };
            _faculty = new Faculty
            {
                Id = 1, Name = "Social Sciences"
            };
            _service = new DepartmentService(_unitOfWork.Object, _facultyRepo.Object, _folderRepo.Object, 
                _deptRepo.Object, _userRepo.Object, _accessLevelRepo.Object);
        }

        [Test]
        public void GetAllDepartmentToList_IfReturnsAllFaculty_ReturnListOfFaculties()
        {
            _deptRepo.Setup(x => x.GetAllToList()).Returns(_departments);

            var result = _service.GetAllDepartmentToList();

            Assert.That(result, Is.EquivalentTo(_departments));
        }

        [Test]
        public void GetDepartmentForPartialView_WhenCalled_ReturnDepartment()
        {
            _deptRepo.Setup(x => x.Get(_department.Id)).Returns(_department);

            var result = _service.GetDepartmentForPartialView(_department.Id);

            Assert.That(result, Is.TypeOf<Department>());

            _deptRepo.Verify(x => x.Get(_department.Id));
        }

        [Test]
        public void GetDepartmentById_WhenCalled_ReturnsDepartment()
        {
            _deptRepo.Setup(x => x.Get(_department.Id)).Returns(_department);

            var result = _service.GetDepartmentById(_department.Id);

            Assert.That(result, Is.EqualTo(_department));

            _deptRepo.Verify(u => u.Get(_department.Id));
        }

        [Test]
        public void SaveDepartment_DepartmentFolderDidNotPreviouslyExist_ReturnServiceResultSucceeded()
        {
            var rootfolder = new Folder {Id = 1, Name = GlobalConstants.RootFolderName};
            var folder = new Folder {Id = 2, ParentId = rootfolder.Id, Name = _department.Name, Department = _department};
            _facultyRepo.Setup(u => u.Get(_department.FacultyId)).Returns(_faculty);
            _accessLevelRepo.Setup(u => u.GetBaseLevel()).Returns(new AccessLevel {Id = 1});
            _folderRepo.Setup(u => u.GetFacultyFolder(_faculty.Name)).Returns(rootfolder);
            _folderRepo.Setup(u => u.Add(folder)).Verifiable();

            var result = _service.SaveDepartment(_department);

            Assert.That(result,Is.EqualTo(ServiceResult.Succeeded));

            _facultyRepo.Verify(u => u.Get(_department.FacultyId));
            _accessLevelRepo.Verify(u => u.GetBaseLevel());
            _folderRepo.Verify(u => u.GetFacultyFolder(_faculty.Name));
            _unitOfWork.Verify(u=> u.Save());
        }

        [Test]
        public void SaveDepartment_DepartmentFolderPreviouslyExist_ReturnServiceResultSucceeded()
        {
            _facultyRepo.Setup(u => u.Get(_department.FacultyId)).Returns(_faculty);
            _folderRepo.Setup(u => u.GetFacultyFolder(_faculty.Name)).Returns(_folder);
            _deptRepo.Setup(u=> u.Add(_department)).Verifiable();

            var result = _service.SaveDepartment(_department);

            Assert.That(result, Is.EqualTo(ServiceResult.Succeeded));

            _facultyRepo.Verify(u => u.Get(_department.FacultyId));
            _folderRepo.Verify(u => u.GetFacultyFolder(_faculty.Name));
            _deptRepo.Verify(u => u.Add(_department));
            _accessLevelRepo.Verify(u => u.GetBaseLevel(),Times.Never);
            _unitOfWork.Verify(u => u.Save());
        }

        [Test]
        public void UpdateDepartment_WhenCalled_ReturnServiceResultSucceeded()
        {
            _deptRepo.Setup(x => x.Update(_department));

            var result = _service.UpdateDepartment(_department);

            Assert.That(result, Is.EqualTo(ServiceResult.Succeeded));

            _deptRepo.Verify(x=>x.Update(_department));
        }

        [Test]
        public void DeleteDepartment_WhenDepartmentContainUsers_ReturnServiceResultProhibited()
        {
            _userRepo.Setup(u => u.Find(It.IsAny<Expression<Func<AppUser, bool>>>())).Returns(_users);

            var result = _service.DeleteDepartment(_department.Id).Result;

            Assert.That(result, Is.EqualTo(ServiceResult.Prohibited));

            _userRepo.Verify(u=>u.Find(It.IsAny<Expression<Func<AppUser, bool>>>()));
        }

        [Test]
        public void DeleteDepartment_WhenDepartmentDoesNotContainsUser_ReturnServiceResultSucceeded()
        {
            _userRepo.Setup(x => x.Find(It.IsAny<Expression<Func<AppUser, bool>>>())).Returns(new List<AppUser>());
            _deptRepo.Setup(x => x.Get(_department.Id)).Returns(_department);
            _deptRepo.Setup(x => x.Remove(_department)).Verifiable();
            _unitOfWork.Setup(x => x.SaveAsync()).Verifiable();
            _folderRepo.Setup(x => x.Find(f => f.DepartmentId == _department.Id)).Returns(_folders);


            var result = _service.DeleteDepartment(_department.Id).Result;

            Assert.That(result, Is.EqualTo(ServiceResult.Succeeded));

            _userRepo.Verify(x => x.Find(It.IsAny<Expression<Func<AppUser, bool>>>()));
            _deptRepo.Verify(x => x.Get(_department.Id));
            _deptRepo.Verify(x => x.Remove(_department));
            _unitOfWork.Verify(x => x.SaveAsync());
        }

        [Test]
        public void GetAllUsersInDepartment_WhenCalled_ReturnsDepartmentUsersViewModel()
        {
            _userRepo.Setup(u=>u.Find(It.IsAny<Expression<Func<AppUser, bool>>>())).Returns(_users);
            _deptRepo.Setup(u => u.Get(_department.Id)).Returns(_department);

            var result = _service.GetAllUsersInDepartment(_department.Id);

            Assert.That(result.Users, Is.EquivalentTo(_users));
            Assert.That(result.Department, Is.EqualTo(_department));
            Assert.That(result, Is.TypeOf<DepartmentUsersViewModel>());

            _userRepo.Verify(u => u.Find(It.IsAny<Expression<Func<AppUser, bool>>>()));
            _deptRepo.Verify(u => u.Get(_department.Id));
        }

        

        [Test]
        [TestCase("Engineering", 1, false)]
        [TestCase("English", 2, true)]
        [TestCase("English", 1, false)]
        public void DepartmentNameCheck_WhenNameExistAndIdIsDifferent_ReturnsTrue(string name, int id, bool expectedResult)
        {
            _deptRepo.Setup(u => u.GetAllToList()).Returns(_departments);
            var result = _service.DepartmentNameCheck(name, id);

            Assert.That(result, Is.EqualTo(expectedResult));

            _deptRepo.Verify(u => u.GetAllToList());
        }
    }
}
