
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
            _repo.Setup(r => r.UserRepo.
                FindWithNavProps(It.IsAny<Expression<Func<AppUser, bool>>>(),
                It.IsAny<Expression<Func<AppUser, object>>>())).Returns(new List<AppUser>());
            var result=_service.DoesUserHasAccessToFolder(new Folder { Name = "", AccessLevelId = 1 },"jljdl");

            Assert.That(result, Is.EqualTo(false));
            _repo.Verify(c => c.UserRepo.FindWithNavProps(It.IsAny<Expression<Func<AppUser, bool>>>(),
                It.IsAny<Expression<Func<AppUser, object>>>()));
        }

        [Test]
        public void DoesUserHasAccessToFolder_FolderBelongsToUsersDepartmentButUserAccessLevelIsLower_ReturnsFalse()
        {
            var userId = "jljlsjljpasdjlfpjpl";
            var user = new AppUser { Id = 1, UserId = userId, DepartmentId = 1, Department = new Department { Id = 1, FacultyId = 1 } };
            var accessDetails = new AccessDetail { AppUserId = user.Id, AccessLevelId = 1 };
            Expression<Func<AppUser,bool>> expression = u => u.UserId == userId;
            Expression<Func<AppUser, object>> include = u => u.Department;
            Folder folder = new Folder() { AccessLevelId = 3,DepartmentId=1, FacultyId = 1 };

            //_repo.Setup(r => r.UserRepo.FindWithNavProps(expression,include))
            //    .Returns(new List<AppUser>(){user});
            _repo.Setup(r => r.UserRepo.
               FindWithNavProps(It.IsAny<Expression<Func<AppUser, bool>>>(),
               It.IsAny<Expression<Func<AppUser, object>>>())).Returns(new List<AppUser> { user});
            _repo.Setup(r => r.AccessDetailsRepo.GetByEmployeeId(user.Id)).Returns(accessDetails);

            var result=_service.DoesUserHasAccessToFolder(folder, userId);

            Assert.That(result, Is.EqualTo(false));
            _repo.Verify(r =>
                r.UserRepo.FindWithNavProps(
                    It.IsAny<Expression<Func<AppUser, bool>>>(),
                    It.IsAny<Expression<Func<AppUser, object>>>()
                    )
                );
            _repo.Verify(r => r.AccessDetailsRepo.GetByEmployeeId(user.Id));

        }
        
    }
}
