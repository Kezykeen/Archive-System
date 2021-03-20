
using Moq;
using NUnit.Framework;
using System;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Entities;
using archivesystemWebUI.Services;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.FolderModels;
using System.Collections.Generic;
using System.Linq;

namespace archivesystemApp.UnitTests
{
    [TestFixture]
    public  class FolderServiceUnitTests
    {
        private Mock<IUnitOfWork> _repo;
        private FolderService _service;
        private CreateFolderViewModel _editModel;
        private Folder _editFolderInDb;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IUnitOfWork>();
            _service = new FolderService(_repo.Object);
            _editModel = new CreateFolderViewModel { Name = GlobalConstants.RootFolderName, Id = 1, AccessLevelId = 1 };
            _editFolderInDb = new Folder { Id = _editModel.Id, Name = _editModel.Name, AccessLevelId = 1 };
        }
        [Test]
        public void DeleteFolder_FolderDoesNotExist_ReturnsFolderServiceResultNotFound()
        {
            _repo.Setup(r => r.FolderRepo.Get(2)).Returns<Folder>(null);
            var result = _service.DeleteFolder(2);
            Assert.That(result, Is.EqualTo(FolderServiceResult.NotFound));
        }

        //Only Faculty and Departmental folders are restricted folders
        [Test]
        public void DeleteFolder_FolderExistButRestricted_ReturnsFolderServiceResultProhibited()
        {
            _repo.Setup(r => r.FolderRepo.Get(2)).Returns(new Folder { Id = 2, IsRestricted = true });
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
            _repo.Setup(r => r.FolderRepo.Get(2)).Returns(new Folder { Id = 2, Name = GlobalConstants.RootFolderName });
            var result = _service.DeleteFolder(2);
            Assert.That(result, Is.EqualTo(FolderServiceResult.Prohibited));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsRootFolderAndUserExist_ReturnsTrue()
        {
            var userData = new UserData { User = new AppUser { DepartmentId = 1 }, UserAccessLevel = 1 };
            var folder = new Folder { AccessLevelId = 1, Department = null, Faculty = null, Name = GlobalConstants.RootFolderName };

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void DoesUserHasAccessToFolder_UserDoesNotExist_ReturnsFalse()
        {
            var userData = new UserData { User = null };
            var folder = new Folder { AccessLevelId = 1, DepartmentId = 1, Department = new Department { Id = 1 } };

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsFacuLtyFolderAndUserIsInFaculty_ReturnsTrue()
        {
            var userData = new UserData { User = new AppUser { Department = new Department { FacultyId = 1 } }, UserAccessLevel = 1 };
            var folder = new Folder { FacultyId = 1, AccessLevelId = 1 };

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsFacuLtyFolderAndUserIsNotInFaculty_ReturnsTrue()
        {
            var userData = new UserData { User = new AppUser { Department = new Department { FacultyId = 3 } }, UserAccessLevel = 1 };
            var folder = new Folder { FacultyId = 1, AccessLevelId = 1 };

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsDepartmentalFolderAndUserIsNotInDepartment_ReturnsTrue()
        {
            var userData = new UserData { User = new AppUser { Department = new Department { Id = 3 } }, UserAccessLevel = 1 };
            var folder = new Folder { DepartmentId = 1, AccessLevelId = 1 };

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderIsDepartmentalFolderAndUserIsInDepartment_ReturnsTrue()
        {
            var userData = new UserData { User = new AppUser { DepartmentId = 1 }, UserAccessLevel = 1 };
            var folder = new Folder { DepartmentId = 1, AccessLevelId = 1, Faculty = null };

            var result = _service.DoesUserHasAccessToFolder(folder, userData);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void Edit_NoAccessLevelId_ReturnFolderServiceResultInvalidModelState()
        {
            _editModel.AccessLevelId = 0;
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.InvalidModelState));
        }

        [Test]
        public void Edit_NewNameIsRootFolderName_ReturnFolderServiceResultAlreadyExist()
        {
            _repo.Setup(x => x.FolderRepo.Get(_editModel.Id))
                .Returns(new Folder {Name=GlobalConstants.RootFolderName,Id=_editModel.Id });
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.AlreadyExist));
        }

        [Test]
        public void Edit_NoFolderName_ReturnFolderServiceResultInvalidModelState()
        {
            _editModel.Name = "";
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.InvalidModelState));
        }

        [Test]
        public void Edit_FolderDoesNotExist_ReturnFolderServiceResultNotFound()
        {
            _repo.Setup(r => r.FolderRepo.Get(_editModel.Id)).Returns<Folder>(null);
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.NotFound));
            _repo.Verify(r => r.FolderRepo.Get(_editModel.Id));
        }

        [Test]
        public void Edit_FolderIsFoundButRestrictedAndNotRootFolder_ReturnFolderServiceResultProhibited()
        {
            _editFolderInDb.IsRestricted = true;
            _editFolderInDb.Name = "dummy name";
            _repo.Setup(r => r.FolderRepo.Get(_editModel.Id)).Returns(_editFolderInDb);
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.Prohibited));
        }

        [Test]
        public void Edit_FolderIsFoundAndNotRestricted_ReturnFolderServiceResultSuccess()
        {
            _editModel.Name = "dummy name";
            _editFolderInDb.Name = _editModel.Name;
            _editFolderInDb.IsRestricted = false;
            _repo.Setup(r => r.FolderRepo.Get(_editModel.Id)).Returns(_editFolderInDb);
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.Success));
            _repo.Verify(r => r.FolderRepo.Get(_editModel.Id));
            _repo.Verify(r => r.Save());
        }

        [Test]
        public void  FilterFolderSubFoldersUsingAccessLevel_WhenCalled_ReturnsFolderWithFilteredSufolders()
        {
            var accessLevel = 2;
            _editFolderInDb.Subfolders= new List<Folder>
            {
                new Folder {AccessLevelId=2},
                new Folder {AccessLevelId=1},
                new Folder {AccessLevelId=2},
                new Folder {AccessLevelId=3},
            };

           var  result=_service.FilterFolderSubFoldersUsingAccessLevel(_editFolderInDb, accessLevel);
            Assert.That(result.Subfolders.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetCreateFolderViewModel_ParentFolderIsNotFound_ReturnNull()
        {
            var parentFolderId = 3;
            var userId = "dummuy id";
            _repo.Setup(r=> r.FolderRepo.Get(parentFolderId)).Returns<Folder>(null);

            var result=_service.GetCreateFolderViewModel(parentFolderId, userId);

            Assert.That(result, Is.EqualTo(null));
            _repo.Verify(r => r.FolderRepo.Get(parentFolderId));
        }

        [Test]
        public void GetCreateFolderViewModel_UserIsNotFound_ReturnNull()
        {
            var parentFolderId = 3;
            var userId = "dummuy id";
            var parentFolder = new Folder { Id = 3 };
            _repo.Setup(r => r.FolderRepo.Get(parentFolderId)).Returns(parentFolder);
            _repo.Setup(r => r.UserRepo.FindWithNavProps(c => c.UserId == userId, _ => _.Department))
                .Returns(new List<AppUser>());
            var result = _service.GetCreateFolderViewModel(parentFolderId, userId);

            Assert.That(result, Is.EqualTo(null));
            _repo.Verify(r => r.FolderRepo.Get(parentFolderId));
            _repo.Verify(r => r.UserRepo.FindWithNavProps(c => c.UserId == userId, _ => _.Department));
        }

        [Test]
        public void GetCreateFolderViewModel_UserAcceesDetailsIsNotFound_ReturnNull()
        {
            var parentFolderId = 3;
            var userId = "dummuy id";
            var user = new AppUser { UserId = userId, Id = 1 };
            var parentFolder = new Folder { Id = 3 };
            _repo.Setup(r => r.FolderRepo.Get(parentFolderId)).Returns(parentFolder);
            _repo.Setup(r => r.UserRepo.FindWithNavProps(c => c.UserId == userId,_ => _.Department))
                .Returns(new List<AppUser> { user });
            _repo.Setup(r => r.AccessDetailsRepo.Find(c => c.AppUserId == user.Id))
                .Returns(new List<AccessDetail> ());
            var result = _service.GetCreateFolderViewModel(parentFolderId, userId);

            Assert.That(result, Is.Null);
            _repo.Verify(r => r.FolderRepo.Get(parentFolderId));
            _repo.Verify(r => r.UserRepo.FindWithNavProps(c => c.UserId == userId, _ => _.Department));
            _repo.Verify(r => r.AccessDetailsRepo.Find(c => c.AppUserId == user.Id));
        }

        [Test]
        public void GetCurrentUserAllowedAccessLevels_UserIsNotFound_ReturnNull()
        {
            var userId = "dummy id";
            var usersList = new List<AppUser>();
            _repo.Setup(r => r.UserRepo.Find(c => c.UserId == userId)).Returns(usersList);

            var result=_service.GetCurrentUserAllowedAccessLevels(userId);

            Assert.That(result,Is.Null);
        }

        [Test]
        public void GetCurrentUserAllowedAccessLevels_AccessDetailsNotFound_ReturnNull()
        {
            var userId = "dummy id";
            var user = new AppUser { UserId = userId, Id = 1 };
            var usersList = new List<AppUser> { user };
            var accesslevels = new List<AccessLevel> { new AccessLevel { Id = 2 }, new AccessLevel{Id=3}};
            _repo.Setup(r => r.UserRepo.Find(c => c.UserId == userId)).Returns(usersList);
            _repo.Setup(r => r.AccessDetailsRepo.Find(x => x.AppUserId == user.Id)).Returns(new List<AccessDetail>());
            _repo.Setup(r => r.AccessLevelRepo.GetAll()).Returns(accesslevels);
            var result = _service.GetCurrentUserAllowedAccessLevels(userId);

            Assert.That(result, Is.Null);
            _repo.Verify(r => r.UserRepo.Find(c => c.UserId == userId));
            _repo.Verify(r => r.AccessDetailsRepo.Find(x => x.AppUserId == user.Id));
            _repo.Verify(r => r.AccessLevelRepo.GetAll());
        }

        [Test]
        public void GetCurrentUserAllowedAccessLevels_AccessDetailsFound_ReturnAccessLevelList()
        {
            var userId = "dummy id";
            var user = new AppUser { UserId = userId, Id = 1 };
            var userDetail = new AccessDetail { Id = 1, AppUserId = user.Id, AccessLevelId = 2 };
            var usersList = new List<AppUser> { user };
            var accessDetails = new List<AccessDetail>{  userDetail};
            var accesslevels = new List<AccessLevel>
                { new AccessLevel { Id = 1 },new AccessLevel { Id = 2 }, new AccessLevel { Id = 3 } };
            var _accesslevels = new List<AccessLevel>
                { new AccessLevel { Id = 1 },new AccessLevel { Id = 2 } };
            _repo.Setup(r => r.UserRepo.Find(c => c.UserId == userId)).Returns(usersList);
            _repo.Setup(r => r.AccessDetailsRepo.Find(x => x.AppUserId == user.Id)).Returns(accessDetails);
            _repo.Setup(r => r.AccessLevelRepo.GetAll()).Returns(accesslevels);

            var result = _service.GetCurrentUserAllowedAccessLevels(userId).ToList();

            Assert.That(result,Is.EquivalentTo(accesslevels.Where(x=> x.Id <= userDetail.AccessLevelId)));
            _repo.Verify(r => r.UserRepo.Find(c => c.UserId == userId));
            _repo.Verify(r => r.AccessDetailsRepo.Find(x => x.AppUserId == user.Id));
            _repo.Verify(r => r.AccessLevelRepo.GetAll());
        }

        [Test]
        public void  GetCurrentUserAccessCode_UserDoesNotExist_ReturnEmptyString()
        {
            var userId = "dummy userId";
            var user = new AppUser { UserId = userId, Id = 1 };
            _repo.Setup( r=> r.UserRepo.GetUserByUserId(userId)).Returns<AppUser>(null);

            var result=_service.GetCurrentUserAccessCode(userId);

            Assert.That(result, Is.EqualTo(""));
            _repo.Verify(r => r.UserRepo.GetUserByUserId(userId));
        }

        public void GetCurrentUserAccessCode_UserDoesNotHaveAccessCode_ReturnEmptyString()
        {
            var userId = "dummy userId";
            var user = new AppUser { UserId = userId, Id = 1 };
            _repo.Setup(r => r.UserRepo.GetUserByUserId(userId)).Returns(user);
            _repo.Setup(r => r.AccessDetailsRepo.Find(x => x.AppUserId == user.Id)).Returns(new List<AccessDetail>());

            var result = _service.GetCurrentUserAccessCode(userId);

            Assert.That(result, Is.EqualTo(""));
            _repo.Verify(r => r.UserRepo.GetUserByUserId(userId));
        }
        public void GetCurrentUserAccessCode_AccessCodeIsFound_ReturnEmptyString()
        {
            var userId = "dummy userId";
            var user = new AppUser { UserId = userId, Id = 1 };
            var userDetails = new AccessDetail { AccessCode = "dummy code" };
            _repo.Setup(r => r.UserRepo.GetUserByUserId(userId)).Returns(user);
            _repo.Setup(r => r.AccessDetailsRepo.Find(x => x.AppUserId == user.Id))
                .Returns(new List<AccessDetail> { userDetails }); 

            var result = _service.GetCurrentUserAccessCode(userId);

            Assert.That(result, Is.EqualTo(userDetails.AccessCode));
            _repo.Verify(r => r.UserRepo.GetUserByUserId(userId));
            _repo.Verify(r => r.AccessDetailsRepo.Find(x => x.AppUserId == user.Id));
        }
    }
}
