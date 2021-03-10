using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;

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
        public ActionResult New()
        {
            return PartialView(new FileMetaVm
            {
                AccessLevel = _unitOfWork.AccessLevelRepo.GetAll()
            });
        }


        public ActionResult Edit()
        {
            return View();
        }
    }
}