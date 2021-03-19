
using Moq;
using NUnit.Framework;
using System;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Entities;
using archivesystemWebUI.Services;
using archivesystemDomain.Services;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using archivesystemWebUI.Models;

namespace archivesystemApp.UnitTests
{
    [TestFixture]
    public partial class FolderServiceUnitTests
    {
        private Mock<IUnitOfWork> _repo;
        private FolderService _service;
        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IUnitOfWork>();
            _service = new FolderService(_repo.Object);
        }
        [Test]
        public void DeleteFolder_FolderDoesNotExist_ReturnsFolderServiceResultNotFound()
        {
            _repo.Setup(r => r.FolderRepo.Get(2)).Returns<Folder>(null);
            var result = _service.DeleteFolder(2);
            Assert.That(result, Is.EqualTo(FolderServiceResult.NotFound)); 
        }

        //Only Faculty and Departmental folders are restrricted folders
        [Test]
        public void DeleteFolder_FolderExistButRestricted_ReturnsFolderServiceResultProhibited()
        {
            _repo.Setup(r => r.FolderRepo.Get(2)).Returns( new Folder {Id=2 ,IsRestricted=true});
            var result = _service.DeleteFolder(2);
            Assert.That(result, Is.EqualTo(FolderServiceResult.Prohibited));
        }

        [Test]
        public void DeleteFolder_FolderExistAndNotRestricted_ReturnsFolderServiceResultSuccesful()
        {
            _repo.Setup(r => r.FolderRepo.Get(2)).Returns(new Folder { Id = 2, IsRestricted = false });
            var result = _service.DeleteFolder(2);
            Assert.That(result, Is.EqualTo(FolderServiceResult.Success));
            _repo.Verify(r => r.FolderRepo.DeleteFolder(2));
            _repo.Verify(r => r.Save());
        }

        [Test]
        public void DeleteFolder_FolderIsRootFolder_ReturnsFolderServiceResultProhibited()
        {
            _repo.Setup(r => r.FolderRepo.Get(2)).Returns(new Folder { Id = 2, Name=GlobalConstants.RootFolderName });
            var result = _service.DeleteFolder(2);
            Assert.That(result, Is.EqualTo(FolderServiceResult.Prohibited));
        }

        [Test]
        public void DoesUserHasAccessToFolder_UserDoesNotExist_ReturnsFalse()
        {
            var userData=new UserData { User = null };
            var folder = new Folder { AccessLevelId = 1, DepartmentId = 1, Department = new Department { Id = 1 } };

            var result =_service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsFacuLtyFolderAndUserIsInFaculty_ReturnsTrue()
        {
            var userData=new UserData{User=new AppUser{Department=new Department {FacultyId=1}},UserAccessLevel=1};
            var folder=new Folder {FacultyId=1,AccessLevelId=1};

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsFacuLtyFolderAndUserIsNotInFaculty_ReturnsTrue()
        {
            var userData=new UserData{User=new AppUser{Department=new Department{FacultyId=3}},UserAccessLevel=1};
            var folder = new Folder {FacultyId=1,AccessLevelId=1};

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsDepartmentalFolderAndUserIsNotInDepartment_ReturnsTrue()
        {
            var userData = new UserData { User = new AppUser{Department=new Department{Id=3}},UserAccessLevel=1};
            var folder = new Folder {DepartmentId=1,AccessLevelId=1 };

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsDepartmentalFolderAndUserIsInDepartment_ReturnsTrue()
        {
            var userData=new UserData{User=new AppUser{ DepartmentId = 1 }, UserAccessLevel = 1 };
            var folder = new Folder { DepartmentId = 1,AccessLevelId=1,Faculty=null};

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(true));
        }

    }
}
