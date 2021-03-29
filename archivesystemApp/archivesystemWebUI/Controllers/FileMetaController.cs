using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using Microsoft.AspNet.Identity;

namespace archivesystemWebUI.Controllers
{
    public class FileMetaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUpsertFile _upsertFile;


        public FileMetaController(IUnitOfWork unitOfWork, IUpsertFile upsertFile )
        {
            _unitOfWork = unitOfWork;
            _upsertFile = upsertFile;
        }
        
        // GET: FileMeta
        public ActionResult New(int folderId)
        {
    

            return PartialView(new FileMetaVm
            {
                AccessLevel = _unitOfWork.AccessLevelRepo.GetAll(),
                FolderId = folderId,
                UploadedById = User.Identity.GetUserId()
                
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(FileMetaVm model, HttpPostedFileBase fileBase)
        {
            if (!ModelState.IsValid)
            {
                model.AccessLevel = _unitOfWork.AccessLevelRepo.GetAll();
                Response.StatusCode = 200;
                return PartialView("New", model);
            }

            if (fileBase !=null)
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
            _unitOfWork.FolderRepo
                .FindWithNavProps(f => f.Id == model.FolderId, _ => _.Files)
                .SingleOrDefault()?.Files.Add(model.File);
            _unitOfWork.Save();
            Response.StatusCode = 201;

            return PartialView("_File", model.File);
        }

      

        public ActionResult GetAllFiles(int folderId)
        {

            var files = _unitOfWork
                .FileRepo
                .FindWithNavProps(_ =>_.FolderId==folderId,
                    _ => _.FileMeta, f =>f.AccessLevel).ToList();
           
            return PartialView(files);
        }

        public FileContentResult GetFile(int id, string fileName)
        {

            var file = _unitOfWork.FileRepo.Get(id);

            if (file==null)
            {
                return null;
            }
            Response.AddHeader("Content-Disposition", "inline; filename=" + fileName);
            return File(file.Content, file.ContentType);
        }
    }
}