using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemApp.UnitTests.WebUIServices
{
    [TestFixture]
    class AccessLevelServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private AccessLevelService _service;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            _unitOfWork.Setup(m => m.AccessLevelRepo.GetAll())
              .Returns(new AccessLevel[] {
                    new AccessLevel{Id = 1, Level = "1", LevelDescription = "Base Level"},
                    new AccessLevel{Id = 2, Level = "2", LevelDescription = "Low Level"},
                    new AccessLevel{Id = 3, Level = "4", LevelDescription = "Average Level"},
                    new AccessLevel{Id = 4, Level = "4", LevelDescription = "High Level"}
              });
            _unitOfWork.Setup(m => m.AccessLevelRepo.Get(1))
                .Returns(new AccessLevel { Id = 1, Level = "1", LevelDescription = "Base Level" });
            _unitOfWork.Setup(m => m.AccessLevelRepo.GetByLevel("1"))
                .Returns(new AccessLevel { Id = 1, Level = "1", LevelDescription = "Base Level" });

            _service = new AccessLevelService(_unitOfWork.Object);

        }

        [Test]
        public void GetAll_ReturnAllObject_ReturnsCountOfObject()
        {            
            // Act
            var result = _service.GetAll();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(result, Is.Not.Empty);
            _unitOfWork.Verify(r => r.AccessLevelRepo.GetAll());
        }

        [Test]
        public void GetById_IdIsNotZero_ReturnsAccessLevel()
        {
            // Arrange
            var id = 1;

            // Act
            var result = _service.GetById(id);

            // Assert
            Assert.That(result, Is.TypeOf<AccessLevel>());
            _unitOfWork.Verify(r => r.AccessLevelRepo.Get(id));
        }

        [Test]
        public void GetById_IdIsZero_ReturnsNull()
        {
            // Arrange
            var id = 0;

            // Act
            var result = _service.GetById(id);

            // Assert
            Assert.That(result == null);
        }        
        
        [Test]
        public void CheckLevel_IfLevelExists_ReturnFalse()
        {
            // Arrange
            var level = "1";

            // Act
            var result = _service.CheckLevel(level);

            // Assert
            Assert.That(result, Is.False);
            _unitOfWork.Verify(r => r.AccessLevelRepo.GetByLevel(level));
        }

        [Test]
        public void CheckLevel_IfLevelDoesNotExist_ReturnTrue()
        {
            // Arrange
            var level = "2";
            

            // Act
            var result = _service.CheckLevel(level);

            // Assert
            Assert.That(result, Is.True);
            _unitOfWork.Verify(r => r.AccessLevelRepo.GetByLevel(level));

        }

        [Test]
        public void Update_WhenCalled_EditsAccessLevel()
        {
            // Arrange
            var accessLevel = new AccessLevel { Id = 1, Level = "1", LevelDescription = "High Level" };

            // Act
            var result = _service.Update(accessLevel);

            // Assert
            _unitOfWork.Verify(r => r.AccessLevelRepo.EditDetails(accessLevel));
        }
    }
}
