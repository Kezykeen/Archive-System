using System;
using System.Collections.Generic;
using System.Linq;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models.FolderModels;
using archivesystemWebUI.Services;
using Moq;
using NUnit.Framework;

namespace archivesystemApp.UnitTests.FolderServiceTests
{
    [TestFixture]
    class FolderService_EditFolder
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
        public void InvalidModelState_ReturnFolderServiceResultInvalidModelState()
        {
            //model state is invalid if the accesslevelId, name or Id of the model is nullorEmpty
            _editModel.AccessLevelId = 0;
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.InvalidModelState));
        }

        [Test]
        public void NewNameIsRootFolderName_ReturnFolderServiceResultAlreadyExist()
        {
            _repo.Setup(x => x.FolderRepo.Get(_editModel.Id))
                .Returns(new Folder { Name = GlobalConstants.RootFolderName, Id = _editModel.Id });
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.AlreadyExist));
        }

        [Test]
        public void NoFolderName_ReturnFolderServiceResultInvalidModelState()
        {
            _editModel.Name = "";
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.InvalidModelState));
        }

        [Test]
        public void FolderDoesNotExist_ReturnFolderServiceResultNotFound()
        {
            _repo.Setup(r => r.FolderRepo.Get(_editModel.Id)).Returns<Folder>(null);
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.NotFound));
            _repo.Verify(r => r.FolderRepo.Get(_editModel.Id));
        }

        [Test]
        public void FolderIsRestricted_ReturnFolderServiceResultProhibited()
        {
            _editFolderInDb.IsRestricted = true;
            _editFolderInDb.Name = "dummy name";
            _repo.Setup(r => r.FolderRepo.Get(_editModel.Id)).Returns(_editFolderInDb);
            var result = _service.Edit(_editModel);
            Assert.That(result, Is.EqualTo(FolderServiceResult.Prohibited));
        }

        [Test]
        public void FolderIsNotRestricted_ReturnFolderServiceResultSuccess()
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

    }
}
