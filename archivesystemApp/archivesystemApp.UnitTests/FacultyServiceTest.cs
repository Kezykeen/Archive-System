using System;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace archivesystemApp.UnitTests
{
    [TestFixture]
    class FacultyServiceTest
    {
        private Mock<IUnitOfWork> _unitOfWork;
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
            _faculty = new Faculty {Id = 1, Name = "dummy faculty", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now};
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
            _service = new FacultyService(_unitOfWork.Object);
        }

        [Test]
        public void GetFacultyData_IfReturnsAllFaculty_ReturnListOfFaculties()
        {
            var faculties = new List<Faculty>
            {
                new Faculty {Name = "Physical science", Id = 1},
                new Faculty {Name = "Social science", Id = 2},
                new Faculty {Name = "Engineering", Id = 3}
            };
            _unitOfWork.Setup(m => m.FacultyRepo.GetAllToList()).Returns(faculties);

            var result = _service.GetFacultyData();

            Assert.That(result, Is.EquivalentTo(faculties));
        }

        [Test]
        public void SaveFaculty_FacultyFolderDidNotPreviouslyExist_ReturnServiceResultSucceeded()
        {
            var rootfolder = new Folder {Id = 1, Name = GlobalConstants.RootFolderName};
            var folders= new List<Folder>{rootfolder};
            _unitOfWork.Setup(u=> u.FolderRepo.FindWithNavProps(x => x.Name == GlobalConstants.RootFolderName,
                x => x.Subfolders)).Returns(folders);
            _unitOfWork.Setup(u => u.FolderRepo.Find(x => x.Name == GlobalConstants.RootFolderName)).Returns(folders);
            _unitOfWork.Setup(x=>x.AccessLevelRepo.GetBaseLevel()).Returns(new AccessLevel{Id=1});

            var result = _service.SaveFaculty(_faculty);

            Assert.That(result,Is.EqualTo(ServiceResult.Succeeded));

            _unitOfWork.Verify(x=> x.AccessLevelRepo.GetBaseLevel());
            _unitOfWork.Verify(x=>x.FolderRepo.Add(rootfolder));
            _unitOfWork.Verify(x=>x.Save());
        }

        [Test]
        public void SaveFaculty_FacultyFolderPreviouslyExist_ReturnServiceResultSucceeded()
        {
            var folders = new List<Folder> {_folder};
            _unitOfWork.Setup(u => u.FolderRepo.FindWithNavProps(x => x.Name == GlobalConstants.RootFolderName,
                x => x.Subfolders)).Returns(folders);
            _unitOfWork.Setup(u => u.FolderRepo.Find(x => x.Name == GlobalConstants.RootFolderName)).Returns(folders);
            _unitOfWork.Setup(x => x.AccessLevelRepo.GetBaseLevel()).Returns(new AccessLevel {Id = 1});
            _unitOfWork.Setup(u=> u.FacultyRepo.Add(_faculty)).Verifiable();

            var result = _service.SaveFaculty(_faculty);

            Assert.That(result, Is.EqualTo(ServiceResult.Succeeded));

            _unitOfWork.Verify(x => x.FacultyRepo.Add(_faculty));
            _unitOfWork.Verify(x => x.AccessLevelRepo.GetBaseLevel(),Times.Never);
            _unitOfWork.Verify(x => x.Save());
        }

        [Test]
        public void UpdateFaculty_WhenCalled_ReturnServiceResultSucceeded()
        {
            _unitOfWork.Setup(x => x.FacultyRepo.Update(_faculty));

            var result = _service.UpdateFaculty(_faculty).Result;

            Assert.That(result, Is.EqualTo(ServiceResult.Succeeded));

            _unitOfWork.Verify(x=>x.FacultyRepo.Update(_faculty));
            _unitOfWork.Verify(x=>x.SaveAsync());
        }

        [Test]
        public void GetFaculty_WhenIdIsNotNull_ReturnFacultyInDb()
        {
            _unitOfWork.Setup(x=>x.FacultyRepo.Get(_faculty.Id)).Returns(_faculty);

            var result = _service.GetFaculty(_faculty.Id);

            Assert.That(result, Is.TypeOf<Faculty>());

            _unitOfWork.Verify(x=>x.FacultyRepo.Get(_faculty.Id));
        }

        [Test]
        public void DeleteFaculty_WhenFacultyContainsDepartment_ReturnServiceResultProhibited()
        {
            _unitOfWork.Setup(u => u.DeptRepo.Find(It.IsAny<Expression<Func<Department, bool>>>())).Returns(_departments);

            var result = _service.DeleteFaculty(_faculty.Id).Result;

            Assert.That(result, Is.EqualTo(ServiceResult.Prohibited));

            _unitOfWork.Verify(x=>x.DeptRepo.Find(It.IsAny<Expression<Func<Department, bool>>>()));
        }

        [Test]
        public void DeleteFaculty_WhenFacultyDoesNotContainsDepartment_ReturnServiceResultSucceeded()
        {
            _unitOfWork.Setup(d => d.DeptRepo.Find(It.IsAny<Expression<Func<Department, bool>>>())).Returns(new List<Department>());
            _unitOfWork.Setup(x => x.FacultyRepo.Get(_faculty.Id)).Returns(_faculty);
            _unitOfWork.Setup(x => x.FacultyRepo.Remove(_faculty)).Verifiable();
            _unitOfWork.Setup(x => x.SaveAsync()).Verifiable();
            _unitOfWork.Setup(x => x.FolderRepo.Find(f => f.FacultyId == _faculty.Id)).Returns(_folders);


            var result = _service.DeleteFaculty(_faculty.Id).Result;

            Assert.That(result, Is.EqualTo(ServiceResult.Succeeded));

            _unitOfWork.Verify(x => x.DeptRepo.Find(It.IsAny<Expression<Func<Department, bool>>>()));
            _unitOfWork.Verify(x => x.FacultyRepo.Get(_faculty.Id));
            _unitOfWork.Verify(x => x.FacultyRepo.Remove(_faculty));
            _unitOfWork.Verify(x => x.SaveAsync());

        }

        [Test]
        public void FacultyNameCheck_IfNameExists_ReturnTrue()
        {
            _unitOfWork.Setup(f => f.FacultyRepo.GetAllToList()).Returns(_faculties);
        }
    }
}