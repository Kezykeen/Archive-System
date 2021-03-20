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

namespace UnitTestProject1.WebUIServices
{
    [TestFixture]
    class AccessLevelServiceTests
    {
        private Mock<IUnitOfWork> unitOfWork;
        private AccessLevelService service;

        [SetUp]
        public void SetUp()
        {
            unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.AccessLevelRepo.GetAll())
              .Returns(new AccessLevel[] {
                    new AccessLevel{Id = 1, Level = "1", LevelDescription = "Base Level"},
                    new AccessLevel{Id = 2, Level = "2", LevelDescription = "Low Level"},
                    new AccessLevel{Id = 3, Level = "4", LevelDescription = "Average Level"},
                    new AccessLevel{Id = 4, Level = "4", LevelDescription = "High Level"}
              });
            service = new AccessLevelService(unitOfWork.Object);

        }

        [Test]
        public void GetAll_ReturnAllObject_ReturnsCountOfObject()
        {            
            // Act
            var result = service.GetAll();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(4));
            Assert.That(result, Is.Not.Empty);

        }
        
        [Test]
        public void GetAll_ContainsLevelOne_ReturnsLevelOne()
        {           
            // Act
            var result = service.GetAll().FirstOrDefault(m => m.Level == "1").Level;

            // Assert
            Assert.That(result, Is.EqualTo("1"));
        }
    }
}
