using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models.FolderModels;
using archivesystemWebUI.Services;
using AutoMapper;
using Moq;
using NUnit.Framework;

namespace archivesystemApp.UnitTests.FolderServiceTests
{
    [TestFixture]
    class FolderService_EditFolder
    {
        private Mock<IFolderServiceRepo> _repo;
        private FolderService _service;
        private CreateFolderViewModel _viewmodel;
        private Folder _folderInDb;
        private Folder _parentFolderInDb;
        private Folder _folder2InDb;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IFolderServiceRepo>();
            _service = new FolderService(_repo.Object);
            _viewmodel = new CreateFolderViewModel { Name = GlobalConstants.RootFolderName, Id =3, AccessLevelId = 1 };
            _folderInDb = new Folder { Id = _viewmodel.Id, Name = _viewmodel.Name, AccessLevelId = 1 };
            _folder2InDb = new Folder { Id = 4, Name = "dummy name 2", AccessLevelId = 1 };
            _parentFolderInDb = new Folder
            {
                Id = 2,
                Name = "dummy parent",
                Subfolders = new List<Folder> { _folderInDb,_folder2InDb }
            };
            Mapper.Initialize(configuration => configuration
                  .CreateMap<EditFolderViewModel, Folder>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.UpdatedAt, opt => opt.UseValue(DateTime.Now)));
        }

        [Test]
        public void InvalidModelState_ReturnFolderServiceResultInvalidModelState()
        {
            //Arrange
            //model state is invalid if the accesslevelId, name or Id of the model is nullorEmpty
            _viewmodel.AccessLevelId = 0;

            //Act
            var result = _service.Edit(_viewmodel);

            //Assert
            Assert.That(result, Is.EqualTo(FolderServiceResult.InvalidModelState));
        }

        [Test]
        public void NewNameIsRootFolderName_ReturnFolderServiceResultAlreadyExist()
        {
            //Arrange
            _repo.Setup(x => x.FolderRepo.Get(_viewmodel.Id))
                .Returns(new Folder { Name = GlobalConstants.RootFolderName, Id = _viewmodel.Id });

            //Act
            var result = _service.Edit(_viewmodel);

            //Assert
            Assert.That(result, Is.EqualTo(FolderServiceResult.AlreadyExist));
        }


        [Test]
        public void FolderDoesNotExist_ReturnFolderServiceResultNotFound()
        {
            //Arrange
            _repo.Setup(r => r.FolderRepo.Get(_viewmodel.Id)).Returns<Folder>(null);

            //Act
            var result = _service.Edit(_viewmodel);

            //Assert
            Assert.That(result, Is.EqualTo(FolderServiceResult.NotFound));
            _repo.Verify(r => r.FolderRepo.Get(_viewmodel.Id));
        }

        [Test]
        public void FolderIsRestricted_ReturnFolderServiceResultProhibited()
        {
            //Arrange
            _folderInDb.IsRestricted = true;
            _folderInDb.Name = "dummy name";

            //Act
            _repo.Setup(r => r.FolderRepo.Get(_viewmodel.Id)).Returns(_folderInDb);

            //Assert
            var result = _service.Edit(_viewmodel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.Prohibited));
        }

        [Test]
        public void FolderIsNotRestrictedAndNewNameDoesNotAlreadyExist_ReturnFolderServiceResultSuccess()
        {
            //Arrange
            _folderInDb.Name = "dummy name 2";
            _viewmodel.Name = "dummy name 3";
            _folderInDb.IsRestricted = false;
            _folderInDb.ParentId = _parentFolderInDb.Id;
            var folderList = new List<Folder> { _parentFolderInDb };

            _repo.Setup(r => r.FolderRepo.Get(_viewmodel.Id)).Returns(_folderInDb);
            _repo.Setup(r => r.FolderRepo.FindWithNavProps(
                It.IsAny<Expression<Func<Folder, bool>>>(),
                It.IsAny<Expression<Func<Folder, object>>>()
                )).Returns(folderList);
           

            //Act
            var result = _service.Edit(_viewmodel);

            //Assert
            Assert.That(result, Is.EqualTo(FolderServiceResult.Success));
            _repo.Verify(r => r.FolderRepo.Get(_viewmodel.Id));
            _repo.Verify(r => r.FolderRepo.FindWithNavProps(
                It.IsAny<Expression<Func<Folder, bool>>>(),
                It.IsAny<Expression<Func<Folder, object>>>()
                ));
            _repo.Verify(r => r.Save());
        }

    }
}
