using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models.FolderModels;
using archivesystemWebUI.Services;
using AutoMapper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemApp.UnitTests.FolderServiceTests
{
    [TestFixture]
    class FolderService_DeleteFolder
    {
        private Mock<IFolderServiceRepo> _repo;
        private FolderService _service;
        private Folder _folder;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IFolderServiceRepo>();
            _service = new FolderService(_repo.Object);
            _folder = new Folder { Id = 2, IsRestricted = true };

        }

        [Test]
        public void FolderDoesNotExist_ReturnsFolderServiceResultNotFound()
        {
            //Arrange
            _repo.Setup(r => r.FolderRepo.Get(_folder.Id)).Returns<Folder>(null);

            //Act
            var result = _service.DeleteFolder(2);

            //Assert
            Assert.That(result, Is.EqualTo(FolderServiceResult.NotFound));
        }

        //Only the root ,Faculty and Departmental folders are restricted folders
        [Test]
        public void FolderIsRestricted_ReturnsFolderServiceResultProhibited()
        {
            //Arrange
            _repo.Setup(r => r.FolderRepo.Get(_folder.Id)).Returns(_folder);

            //Act
            var result = _service.DeleteFolder(_folder.Id);

            //Assert
            Assert.That(result, Is.EqualTo(FolderServiceResult.Prohibited));
        }

        [Test]
        public void FolderIsNotRestricted_ReturnsFolderServiceResultSuccesful()
        {
            //Arrange
            _folder.IsRestricted = false;
            _repo.Setup(r => r.FolderRepo.Get(_folder.Id)).Returns(_folder );

            //Act
            var result = _service.DeleteFolder(_folder.Id);

            //Assert
            Assert.That(result, Is.EqualTo(FolderServiceResult.Success));
            _repo.Verify(r => r.FolderRepo.DeleteFolder(_folder.Id));
            _repo.Verify(r => r.Save());
        }

        [Test]
        public void FolderIsRootFolder_ReturnsFolderServiceResultProhibited()
        {
            //Arrange
            _folder.Name = GlobalConstants.RootFolderName;
            _repo.Setup(r => r.FolderRepo.Get(_folder.Id)).Returns(_folder);

            //Act
            var result = _service.DeleteFolder(_folder.Id);

            //Asseert
            Assert.That(result, Is.EqualTo(FolderServiceResult.Prohibited));
        }
    }
}
