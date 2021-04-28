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

namespace archivesystemApp.UnitTests.FacultyServiceTests
{
    [TestFixture]
    class FacultyServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IFolderRepo> _folderRepo;
        private Mock<IDeptRepository> _deptRepo;
        private Mock<IFacultyRepository> _facultyRepo;
        private Mock<IAccessLevelRepository> _accessLevelRepo;
        private Faculty _faculty;
        private List<Faculty> _faculties;
        private List<Department> _departments;
        private Folder _folder;
        private List<Folder> _folders;
        private FacultyService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _folderRepo = new Mock<IFolderRepo>();
            _deptRepo = new Mock<IDeptRepository>();
            _facultyRepo = new Mock<IFacultyRepository>();
            _accessLevelRepo = new Mock<IAccessLevelRepository>();
            _faculty = new Faculty {Id = 1, Name = "Engineering", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now};
            _faculties = new List<Faculty>
            {
                _faculty
            };
            _departments = new List<Department>
            {
                new Department {Id = 2, Name = "Mathematics", FacultyId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now}
            };
            _folder =  new Folder
            {
                Id = 1, Name = GlobalConstants.RootFolderName, Subfolders = new List<Folder>
                {
                    new Folder {Name = _faculty.Name, ParentId = 1}
                }
            };
            _folders = new List<Folder>
            {
                _folder
            };
            _service = new FacultyService(_unitOfWork.Object, _facultyRepo.Object, _folderRepo.Object,
                _deptRepo.Object, _accessLevelRepo.Object);
        }

        [Test]
        public void GetAllFacultiesToLIst_WhenCalled_ReturnListOfFaculties()
        {
            _facultyRepo.Setup(m => m.GetAllToList()).Returns(_faculties);

            var result = _service.GetAllFacultiesToList();

            Assert.That(result, Is.EquivalentTo(_faculties));
        }

        [Test]
        public void SaveFaculty_FacultyFolderDidNotPreviouslyExist_ReturnServiceResultSucceeded()
        {
            var rootfolder = new Folder {Id = 1, Name = GlobalConstants.RootFolderName};
            var folders= new List<Folder>{rootfolder};
            _folderRepo.Setup(u=> u.FindWithNavProps(x => x.Name == GlobalConstants.RootFolderName,
                x => x.Subfolders)).Returns(folders);
            _folderRepo.Setup(u => u.Find(x => x.Name == GlobalConstants.RootFolderName)).Returns(folders);
            _accessLevelRepo.Setup(u=>u.GetBaseLevel()).Returns(new AccessLevel{Id=1});

            var (serviceResult, message) = _service.SaveFaculty(_faculty);

            Assert.That(serviceResult, Is.EqualTo(ServiceResult.Succeeded));
            Assert.That(message, Is.EqualTo(""));

            _accessLevelRepo.Verify(u=> u.GetBaseLevel());
            _unitOfWork.Verify(u=>u.Save());
        }

        [Test]
        public void SaveFaculty_FacultyFolderPreviouslyExist_ReturnServiceResultSucceeded()
        {
            _folderRepo.Setup(u => u.FindWithNavProps(x => x.Name == GlobalConstants.RootFolderName,
                x => x.Subfolders)).Returns(_folders);
            _folderRepo.Setup(u => u.Find(x => x.Name == GlobalConstants.RootFolderName)).Returns(_folders);
            _accessLevelRepo.Setup(u => u.GetBaseLevel()).Returns(new AccessLevel {Id = 1});
            _facultyRepo.Setup(u=> u.Add(_faculty)).Verifiable();
            _facultyRepo.Setup(u => u.GetAllToList()).Returns(_faculties);

            var(serviceResult, message) = _service.SaveFaculty(_faculty);

            Assert.That(serviceResult, Is.EqualTo(ServiceResult.Succeeded));
            Assert.That(message, Is.EqualTo(""));

            _facultyRepo.Verify(x => x.Add(_faculty));
            _facultyRepo.Verify(u => u.GetAllToList());
            _accessLevelRepo.Verify(x => x.GetBaseLevel(),Times.Never);
            _unitOfWork.Verify(x => x.Save());
        }

        [Test]
        public void UpdateFaculty_WhenCalled_ReturnServiceResultSucceeded()
        {
            _facultyRepo.Setup(x => x.Update(_faculty));
            _facultyRepo.Setup(u => u.GetAllToList()).Returns(_faculties);

            var(serviceResult, message) = _service.UpdateFaculty(_faculty);

            Assert.That(serviceResult, Is.EqualTo(ServiceResult.Succeeded));
            Assert.That(message, Is.EqualTo(""));

            _facultyRepo.Verify(x=>x.Update(_faculty));
            _facultyRepo.Verify(u => u.GetAllToList());
        }

        [Test]
        public void GetFacultyForPartialView_WhenCalled_ReturnFaculty()
        {
            _facultyRepo.Setup(x=>x.Get(_faculty.Id)).Returns(_faculty);

            var result = _service.GetFacultyForPartialView(_faculty.Id);

            Assert.That(result, Is.TypeOf<Faculty>());

            _facultyRepo.Verify(x=>x.Get(_faculty.Id));
        }

        [Test]
        public void DeleteFaculty_WhenFacultyContainsDepartment_ReturnServiceResultProhibited()
        {
            _deptRepo.Setup(u => u.Find(It.IsAny<Expression<Func<Department, bool>>>())).Returns(_departments);

            var result = _service.DeleteFaculty(_faculty.Id).Result;

            Assert.That(result, Is.EqualTo(ServiceResult.Prohibited));

            _deptRepo.Verify(x=>x.Find(It.IsAny<Expression<Func<Department, bool>>>()));
        }

        [Test]
        public void DeleteFaculty_WhenFacultyDoesNotContainsDepartment_ReturnServiceResultSucceeded()
        {
            _deptRepo.Setup(x => x.Find(It.IsAny<Expression<Func<Department, bool>>>())).Returns(new List<Department>());
            _facultyRepo.Setup(x => x.Get(_faculty.Id)).Returns(_faculty);
            _facultyRepo.Setup(x => x.Remove(_faculty)).Verifiable();
            _unitOfWork.Setup(x => x.SaveAsync()).Verifiable();
            _folderRepo.Setup(x => x.Find(f => f.FacultyId == _faculty.Id)).Returns(_folders);

            var result = _service.DeleteFaculty(_faculty.Id).Result;

            Assert.That(result, Is.EqualTo(ServiceResult.Succeeded));

            _deptRepo.Verify(x => x.Find(It.IsAny<Expression<Func<Department, bool>>>()));
            _facultyRepo.Verify(x => x.Get(_faculty.Id));
            _facultyRepo.Verify(x => x.Remove(_faculty));
            _unitOfWork.Verify(x => x.SaveAsync());
        }

        [Test]
        public void GetAllDepartmentsInFaculty_WhenCalled_ReturnsFacultyDepartmentsViewModel()
        {
            _deptRepo.Setup(u=>u.Find(It.IsAny<Expression<Func<Department, bool>>>())).Returns(_departments);
            _facultyRepo.Setup(u => u.Get(_faculty.Id)).Returns(_faculty);

            var result = _service.GetAllDepartmentsInFaculty(_faculty.Id);

            Assert.That(result.Departments, Is.EquivalentTo(_departments));
            Assert.That(result.Faculty, Is.EqualTo(_faculty));
            Assert.That(result, Is.TypeOf<FacultyDepartmentsViewModel>());

            _deptRepo.Verify(u => u.Find(It.IsAny<Expression<Func<Department, bool>>>()));
            _facultyRepo.Verify(u => u.Get(_faculty.Id));
        }

        [Test]
        public void GetFacultyById_WhenCalled_ReturnsFaculty()
        {
            _facultyRepo.Setup(u => u.Get(_faculty.Id)).Returns(_faculty);

            var result = _service.GetFacultyById(_faculty.Id);

            Assert.That(result, Is.EqualTo(_faculty));

            _facultyRepo.Verify(u=>u.Get(_faculty.Id));
        }

        [Test]
        [TestCase("Engineering", 1, false)]
        [TestCase("Engineering", 2, true)]
        [TestCase("English", 1, false)]
        public void FacultyNameCheck_WhenNameExistAndIdIsDifferent_ReturnsTrue(string name, int id, bool expectedResult)
        {
            _facultyRepo.Setup(u => u.GetAllToList()).Returns(_faculties);
            var result = _service.DoesFacultyNameExist(name, id);

            Assert.That(result, Is.EqualTo(expectedResult));

            _facultyRepo.Verify(u => u.GetAllToList());
        }
    }
}