using System;
using System.Linq;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using archivesystemWebUI.Services;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;

namespace archivesystemUnitTest
{
    [TestFixture()]
    public class UserAccessServiceTests
    {             
        [Test()]
        public void AddUser_UserAdded_ReturnTrue()
        {
            // Arrange
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(m => m.AccessDetailsRepo.GetAccessDetails()).Returns(new AccessDetail[] {
                new AccessDetail { Id =1, AppUserId= 1, AccessLevelId = 1, AccessCode = "ajkcnkj", Status = Status.Active},
                new AccessDetail { Id =2, AppUserId = 12, AccessLevelId = 22, AccessCode = "78ckled", Status = Status.Inactive},
                new AccessDetail { Id =3, AppUserId = 13, AccessLevelId = 33, AccessCode = "a98jkcnkj", Status = Status.Active},
                new AccessDetail { Id =4, AppUserId = 14, AccessLevelId = 44, AccessCode = "ajk0987cnkj", Status = Status.Inactive},
                new AccessDetail { Id =5, AppUserId = 15, AccessLevelId = 55, AccessCode = "ajclke3kcnkj", Status = Status.Active},
                new AccessDetail { Id =6, AppUserId = 16, AccessLevelId = 66, AccessCode = "aj,m2kcnkj", Status = Status.Inactive}
            });
            mock.Setup(m => m.UserRepo.GetAll()).Returns(new AppUser[] { 
            new AppUser {Id = 1, Name = "Muna", Email = "marvelousfrank5@gmail.com"}
            });

            var model = new AddUserToAccessViewModel { AccessLevel = 5, Email = "marvelousfrank5@gmail.com" };


            // Act
            var result = new UserAccessService(mock.Object);
           // result.AddUser(model);

            // Assert
           Assert.That(result.AccessDetails.Count() == 7);
        }

       
    }
}
