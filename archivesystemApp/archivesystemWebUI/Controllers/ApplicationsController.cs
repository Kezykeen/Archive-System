using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;
using Microsoft.AspNet.Identity;

namespace archivesystemWebUI.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUpsertFile _upsertFile;

        public ApplicationsController(IUnitOfWork unitOfWork, IUpsertFile upsertFile)
        {
            _unitOfWork = unitOfWork;
            _upsertFile = upsertFile;
        }
        
        // GET: Applications
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult New()
        {
            var currentUserId = User.Identity.GetUserId();


            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var model = new ApplicationVm
            {
                UserId = user?.Id ?? 0
            };
            if (User.IsInRole("Student"))
            {
                model.ApplicationTypes = _unitOfWork.TicketRepo.Find(t => t.Designation == Designation.Student);
                model.DepartmentId = user?.DepartmentId ?? 0;
            }else if (User.IsInRole("Alumni"))
            {
                model.ApplicationTypes = _unitOfWork.TicketRepo.Find(t => t.Designation == Designation.Alumni);
                model.Departments = _unitOfWork.DeptRepo.GetAll();

            }else if (User.IsInRole("Staff"))
            {
                _unitOfWork.TicketRepo.Find(t => t.Designation == Designation.Staff);
                model.Departments = _unitOfWork.DeptRepo.GetAll();
            }

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ApplicationVm model, HttpPostedFileBase fileBase)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();

            if (!ModelState.IsValid)
            {

                if (User.IsInRole("Student"))
                {
                    model.ApplicationTypes = _unitOfWork.TicketRepo.Find(t => t.Designation == Designation.Student);
                    model.DepartmentId = user?.DepartmentId ?? 0;
                }
                else if (User.IsInRole("Alumni"))
                {
                    model.ApplicationTypes = _unitOfWork.TicketRepo.Find(t => t.Designation == Designation.Alumni);
                    model.Departments = _unitOfWork.DeptRepo.GetAll();

                }
                else if (User.IsInRole("Staff"))
                {
                    _unitOfWork.TicketRepo.Find(t => t.Designation == Designation.Staff);
                    model.Departments = _unitOfWork.DeptRepo.GetAll();
                }
                return PartialView("New", model);
            }

            if (fileBase != null)
            {
              model.Attachment = _upsertFile.Save(model.Attachment, fileBase);
            }
          
            var application = Mapper.Map<Application>(model);
            application.Title =
                $"{user?.TagId}_{_unitOfWork.TicketRepo.Get(model.ApplicationTypeId).Name}_{DateTime.Now:yy/MM/dd}";
            application.Receivers.Add(
                
                new ApplicationReceiver
                {
                    ReceiverId = model.DepartmentId,
                    TimeSent = DateTime.Now
                }
            );
            _unitOfWork.ApplicationRepo.Add(application);
            _unitOfWork.Save();

            return Json(new { saved = true });

        }
    }
}