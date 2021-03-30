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
      
        private readonly IFileService _fileService;
        private readonly IAccessLevelService _accessLevelService;


        public FileMetaController(IFileService fileService , IAccessLevelService accessLevelService)
        {
            _fileService = fileService;
            _accessLevelService = accessLevelService;
        }
        
        // GET: FileMeta
        public ActionResult New(int folderId)
        {
    

            return PartialView(new FileMetaVm
            {
                AccessLevel =_accessLevelService.GetAll(),
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
                model.AccessLevel = _accessLevelService.GetAll();
                Response.StatusCode = 200;
                return PartialView("New", model);
            }

            var file = _fileService.Create(model, fileBase);

            if (file.save)
            {
                Response.StatusCode = 201;
                return PartialView("_File", model.File);
            }

            ModelState.AddModelError("", "An Error Occurred!");
            return PartialView("New", model);
        }

      

        public ActionResult GetAllFiles(int folderId)
        {

            var files = _fileService.GetFiles(folderId);
           
            return PartialView(files);
        }

        public FileContentResult GetFile(int id, string fileName)
        {
            var file = _fileService.GetFile(id, fileName);

            if (file==null)
            {
                return null;
            }
            Response.AddHeader("Content-Disposition", "inline; filename=" + fileName);
            return File(file.Content, file.ContentType);
        }
    }
}