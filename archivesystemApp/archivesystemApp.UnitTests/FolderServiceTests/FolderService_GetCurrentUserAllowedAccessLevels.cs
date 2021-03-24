using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Entities;
using archivesystemWebUI.Services;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.FolderModels;

namespace archivesystemApp.UnitTests.FolderServiceTests
{
    [TestFixture]
    class FolderService_GetCurrentUserAllowedAccessLevels
    {
        private Mock<IUnitOfWork> _repo;
        private FolderService _service;
        private int _parentFolderId = 3;
        private string _userId;
        private Folder _parentFolder;
        private AppUser _user;
        private List<AccessLevel> _accesslevels;
        private List<AppUser> _usersList;

        [SetUp]
        public void Setup()
        {
            _parentFolder = new Folder { Id = 3 };
            _parentFolderId = 3;
            _userId = "dummy id";
            _repo = new Mock<IUnitOfWork>();
            _service = new FolderService(_repo.Object);
            _user = new AppUser { UserId = _userId, Id = 1 };
            _repo.Setup(r => r.FolderRepo.Get(_parentFolderId)).Returns(_parentFolder);
            _accesslevels = new List<AccessLevel>
                { new AccessLevel { Id = 1 },new AccessLevel { Id = 2 }, new AccessLevel { Id = 3 } };
            _usersList = new List<AppUser>();
        }

        [Test]
        public void UserIsNotFound_ReturnNull()
        {
            var dummyId = _userId;
            _repo.Setup(r => r.UserRepo.Find(c => c.UserId == _userId)).Returns(_usersList);

            var result = _service.GetCurrentUserAllowedAccessLevels(_userId);

            Assert.That(result, Is.Null);
            _repo.Verify(r => r.UserRepo.Find(c => c.UserId ==dummyId));
        }

        [Test]
        public void UserhasNotBeingAssignedToAnAccessLevel_ReturnNull()
        {
            _usersList.Add(_user);
            var dummyId = "dummy id"; // must be same as _userId;
            var dummyAppUserId = 1; //must be same as _user.Id
            _repo.Setup(r => r.UserRepo.Find(c => c.UserId == dummyId)).Returns(_usersList);
            _repo.Setup(r => r.AccessDetailsRepo.Find(x => x.AppUserId == dummyAppUserId)).Returns(new List<AccessDetail>());
            _repo.Setup(r => r.AccessLevelRepo.GetAll()).Returns(_accesslevels);

            var result = _service.GetCurrentUserAllowedAccessLevels(_userId);

            Assert.That(result, Is.Null);
            _repo.Verify(r => r.UserRepo.Find(c => c.UserId == dummyId));
            _repo.Verify(r => r.AccessDetailsRepo.Find(x => x.AppUserId == dummyAppUserId));
            _repo.Verify(r => r.AccessLevelRepo.GetAll());
        }
        

        [Test]
        public void GetCurrentUserAllowedAccessLevels_AccessDetailsFound_ReturnAccessLevelList()
        {
            var userDetail = new AccessDetail { Id = 1, AppUserId = _user.Id, AccessLevelId = 2 };
            _usersList.Add( _user);
            var accessDetails = new List<AccessDetail> { userDetail };
            _repo.Setup(r => r.UserRepo.Find(c => c.UserId == _userId)).Returns(_usersList);
            _repo.Setup(r => r.AccessDetailsRepo.Find(x => x.AppUserId == _user.Id)).Returns(accessDetails);
            _repo.Setup(r => r.AccessLevelRepo.GetAll()).Returns(_accesslevels);

            var result = _service.GetCurrentUserAllowedAccessLevels(_userId).ToList();

            Assert.That(result, Is.EquivalentTo(_accesslevels.Where(x => x.Id <= userDetail.AccessLevelId)));
            _repo.Verify(r => r.UserRepo.Find(c => c.UserId == _userId));
            _repo.Verify(r => r.AccessDetailsRepo.Find(x => x.AppUserId == _user.Id));
            _repo.Verify(r => r.AccessLevelRepo.GetAll());
        }

    }
}
