﻿using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using archivesystemWebUI.Services;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace archivesystemApp.UnitTests.WebUIServices
{
    [TestFixture]
    class UserAccessServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IAccessCodeGenerator> _accessCodeGenerator;
        private Mock<IAccessDetailsRepository> _accessDetailsRepo;
        private Mock<IUserRepository> _userRepo ;
        private Mock<IAccessLevelRepository> _accessLevelRepo;
        private string _email;
        private AccessDetail _accessDetails;
        private UserAccessService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _accessCodeGenerator = new Mock<IAccessCodeGenerator>();
            _accessDetailsRepo = new Mock<IAccessDetailsRepository>();
            _userRepo = new Mock<IUserRepository>();
            _accessLevelRepo = new Mock<IAccessLevelRepository>();
            _email = "marvelousfrank5@gmail.com";
            _accessDetails = new AccessDetail { Id = 1, AccessCode = "gkuy23rfd", AccessLevelId = 5, Status = Status.Active };

            _accessDetailsRepo.Setup(m => m.GetAccessDetails())
              .Returns(new AccessDetail[] {
                    new AccessDetail{Id = 1, AccessCode = "gkuy23rfd", AccessLevelId = 5, Status = Status.Active },
                    new AccessDetail{Id = 2, AccessCode = "23rfd", AccessLevelId = 1, Status = Status.Active }
             });
            _accessDetailsRepo.Setup(m => m.Get(1)).Returns(_accessDetails);
            _accessDetailsRepo.Setup(m => m.Remove(_accessDetails));
            _accessDetailsRepo.Setup(m => m.GetByAppUserId(5))
              .Returns(new AccessDetail{Id = 1, AppUserId = 5,  AccessCode = "gkuy23rfd", AccessLevelId = 5, Status = Status.Active });
            _userRepo.Setup(m => m.GetUserByMail(_email))
                .Returns(new AppUser { Id = 1, Name = "Marvelous", Email = _email });
            _userRepo.Setup(m =>m.Get(1))
                .Returns(new AppUser { Id = 1, Name = "Marvelous", Email = _email });

            _service = new UserAccessService(_unitOfWork.Object, _accessCodeGenerator.Object, _accessDetailsRepo.Object,
                _userRepo.Object, _accessLevelRepo.Object);
        }

        [Test]
        public void AddUser_WhenModelValid_ReturnsSuccess()
        {
            // Arrange
            var model = new AddUserToAccessViewModel { Email = _email, AccessLevel = 1 };

            // Act
            var result = _service.AddUser(model).Result;

            // Assert
            Assert.That(result.Item1, Is.EqualTo("success"));
            Assert.That(result.Item2, Is.EqualTo(null));
        }

        [Test]
        public void AddUser_WhenModelInvalid_ReturnsFailure()
        {
            // Arrange
            var model = new AddUserToAccessViewModel { Email = null, AccessLevel = 1 };

            // Act
            var result = _service.AddUser(model).Result;

            // Assert
            Assert.That(result.Item1, Is.EqualTo("failure"));
            Assert.That(result.Item2, Is.InstanceOf<Exception>());
        }
        
        [Test]
        public void AddUser_VerifyRepoCalls()
        {
            // Arrange
            var model = new AddUserToAccessViewModel { Email = _email, AccessLevel = 1 };

            // Act
            var result = _service.AddUser(model).Result;

            // Assert
            _userRepo.Verify(m => m.GetUserByMail(_email));
            _unitOfWork.Verify(m => m.SaveAsync());

        }

        [Test]
        public void UpdateUser_WhenModelValid_ReturnsSuccess()
        {
            // Arrange
            var model = new EditUserViewModel { RegenerateCode = CodeStatus.No, AccessDetails = new AccessDetail { Id = 1, AccessCode = "gkuy23rfd", AccessLevelId = 5, Status = Status.Active } };

            // Act
            var result = _service.UpdateUser(model).Result;

            // Assert
            Assert.That(result.Item1, Is.EqualTo("success"));
            Assert.That(result.Item2, Is.EqualTo(null));
        }

        [Test]
        public void UpdateUser_WhenModelInValid_ReturnsFailure()
        {
            // Act
            var result = _service.UpdateUser(null).Result;

            // Assert
            Assert.That(result.Item1, Is.EqualTo("failure"));
            Assert.That(result.Item2, Is.InstanceOf<Exception>());
        }
        
        
        [Test]
        public void UpdateUser_VerifyRepoCalls()
        {
            // Arrange
            var model = new EditUserViewModel { RegenerateCode = CodeStatus.No, AccessDetails = _accessDetails};

            // Act
            var result = _service.UpdateUser(model).Result;

            // Assert
            _accessDetailsRepo.Verify(r => r.EditDetails(model.AccessDetails));
            _unitOfWork.Verify(r => r.SaveAsync());
        }

        [Test]
        public void Delete_WhenCalled_RemovesAccessDetails()
        {
            // Arrange
            var id = 1; 

            // Act
            var result = _service.Delete(id).Result;

            // Assert
            Assert.That(result, Is.EqualTo("success"));
        }

        [Test]
        public void Delete_VerifyRepoCalls()
        {
            // Arrange
            var id = 1;

            // Act
            var result = _service.Delete(id).Result;

            // Assert
            _accessDetailsRepo.Verify(r => r.Remove(_accessDetails));
            _unitOfWork.Verify(r => r.SaveAsync());
        }

        [Test]
        public void GetUserByEmail_UserExists_ReturnsAppUser()
        {
            // Act
            var result = _service.GetUserByEmail(_email);

            // Assert
            Assert.That(result, Is.TypeOf<AppUser>());
            _userRepo.Verify(r => r.GetUserByMail(_email));
        }
        
        [Test]
        public void GetUserByEmail_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var unknownEmail = "muna@gmail.com";

            // Act
            var result = _service.GetUserByEmail(unknownEmail);

            // Assert
            Assert.That(result, Is.Null);
            _userRepo.Verify(r => r.GetUserByMail(unknownEmail));
        }

        [Test]
        public void GetByAppUserId_UserIdExists_ReturnsAccessDetail()
        {
            // Arrange
            var id = 5;

            // Act
            var result = _service.GetByAppUserId(id);

            // Assert
            Assert.That(result, Is.TypeOf<AccessDetail>());
            _accessDetailsRepo.Verify(r => r.GetByAppUserId(id));
        }
        
        [Test]
        public void GetByAppUserId_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var id = 3;

            // Act
            var result = _service.GetByAppUserId(id);

            // Assert
            Assert.That(result, Is.Null);
            _accessDetailsRepo.Verify(r => r.GetByAppUserId(id));
        }
    }
}
