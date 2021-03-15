﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Models;
using Microsoft.AspNet.Identity;

namespace archivesystemWebUI.Controllers
{
    public class FileMetaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public FileMetaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                UpsertFile.Save(model, fileBase);
            }

            model.File.FileMeta = new FileMeta
            {
                Title = model.Title,
                UploadedById = model.UploadedById,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
         
            model.File.AccessLevelId = model.AccessLevelId;
            model.File.CreatedAt = DateTime.Now;
            model.File.UpdatedAt = DateTime.Now;

          _unitOfWork.FolderRepo
                .FindWithNavProps(f => f.Id == model.FolderId, _ => _.Files)
                .SingleOrDefault()?.Files.Add(model.File);
            _unitOfWork.Save();
            Response.StatusCode = 201;

            return PartialView("_File", model.File);
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult GetAllFiles(int folderId)
        {

            var files = _unitOfWork
                .FileRepo
                .FindWithNavProps(_ =>_.FolderId==folderId,
                    _ => _.FileMeta, f =>f.AccessLevel).ToList();
           
            return PartialView(files);
        }
    }
}