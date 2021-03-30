using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Services
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFolderRepo _folderRepo;
        private readonly IUpsertFile _upsertFile;
        private readonly IFileRepo _fileRepo;

        public FileService(
            IUnitOfWork unitOfWork,
            IFolderRepo folderRepo,
            IUpsertFile upsertFile,
            IFileRepo fileRepo)
        {
            _unitOfWork = unitOfWork;
            _folderRepo = folderRepo;
            _upsertFile = upsertFile;
            _fileRepo = fileRepo;
        }

        public (bool save, FileMetaVm model) Create(FileMetaVm model, HttpPostedFileBase fileBase)
        {
            if (fileBase != null)
            {
                model.File = _upsertFile.Save(model.File, fileBase);
            }

            model.File.FileMeta = new FileMeta
            {
                Title = model.Title,
                UploadedById = model.UploadedById,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };


            model.File.AccessLevelId = model.AccessLevelId;
            _folderRepo
                .FindWithNavProps(f => f.Id == model.FolderId, _ => _.Files)
                .SingleOrDefault()?.Files.Add(model.File);
            _unitOfWork.Save();
            return (true, model);

        }

        public ICollection<File> GetFiles(int folderId)
        {
            var files = 
                _fileRepo
                .FindWithNavProps(_ => _.FolderId == folderId,
                    _ => _.FileMeta, f => f.AccessLevel).ToList();

            return files;

        }

        public File Details(int id)
        {
            return _fileRepo.FindWithNavProps(f => f.Id == id, _ => _.FileMeta, _ => _.FileMeta.UploadedBy, _ => _.Folder).SingleOrDefault();
        }
        public File GetFile(int id, string fileName)
        {
            return _fileRepo.Get(id);
        }
    }
}