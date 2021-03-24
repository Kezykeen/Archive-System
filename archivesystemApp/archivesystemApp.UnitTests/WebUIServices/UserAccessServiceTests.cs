using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
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
        private Mock<IEmailSender> _emailSender;
        private string _email;
        private UserAccessService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _emailSender = new Mock<IEmailSender>();
            _email = "marvelousfrank5@gmail.com";

            _unitOfWork.Setup(m => m.AccessDetailsRepo.GetAccessDetails())
              .Returns(new AccessDetail[] {
                    new AccessDetail{Id = 1, AccessCode = "gkuy23rfd", AccessLevelId = 5, Status = Status.Active },
                    new AccessDetail{Id = 2, AccessCode = "23rfd", AccessLevelId = 1, Status = Status.Active }
             });
            _unitOfWork.Setup(m => m.AccessDetailsRepo.Get(1))
              .Returns(new AccessDetail{Id = 1, AccessCode = "gkuy23rfd", AccessLevelId = 5, Status = Status.Active });
            _unitOfWork.Setup(m => m.AccessDetailsRepo.GetByAppUserId(1))
              .Returns(new AccessDetail{Id = 1, AccessCode = "gkuy23rfd", AccessLevelId = 5, Status = Status.Active });
            _unitOfWork.Setup(m => m.UserRepo.GetUserByMail(_email))
                .Returns(new AppUser { Id = 1, Name = "Marvelous", Email = "marvelousfrank5@gmail.com" });
            _unitOfWork.Setup(m =>m.UserRepo.Get(1))
                .Returns(new AppUser { Id = 1, Name = "Marvelous", Email = "marvelousfrank5@gmail.com" });


            _service = new UserAccessService(_unitOfWork.Object, _emailSender.Object);

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
        public void Delete_IdExists_RemovesAccessDetails()
        {
            // Arrange
            var id = 1;

            // Act
            var result = _service.Delete(id).Result;

            // Assert
            Assert.That(result, Is.EqualTo("success"));

        }
        
        [Test]
        public void GetUserByEmail_WhenCalled_ReturnsAppUser()
        {
            // Act
            var result = _service.GetUserByEmail(_email);

            // Assert
            Assert.That(result, Is.TypeOf<AppUser>());
        }
        
        


        [Test]
        public void GetById_WhenCalled_ReturnsAccessDetail()
        {
            // Act
            var result = _service.GetByAppUserId(1);

            // Assert
            Assert.That(result, Is.TypeOf<AccessDetail>());
        }

        
    }
}
