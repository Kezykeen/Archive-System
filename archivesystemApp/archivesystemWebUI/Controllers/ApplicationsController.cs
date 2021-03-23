using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Ninject.Infrastructure.Language;
using WebGrease.Css.Extensions;

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


        public ActionResult Incoming()
        {
            return View();
        }
        public ActionResult PendingApproval()
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
                $"{user?.TagId}_{_unitOfWork.TicketRepo.Get(model.ApplicationTypeId).Acronym}_{DateTime.Now:yy/MM/dd}";
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

        public ActionResult Accept(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo.FindWithNavProps(a => a.Id == id, _=> _.Receivers).SingleOrDefault();
            if (application == null) return HttpNotFound();
            var appReceived = application.Receivers.SingleOrDefault(r => r.ReceiverId == user?.DepartmentId);
            if (appReceived == null || appReceived.Received != null) return RedirectToAction("Details", new {id});
            appReceived.Received = true;
            _unitOfWork.Save();
            return  RedirectToAction("Details", new{id});

          
        }


        public ActionResult AssignUsers(int appId)
        {
            return PartialView(new AssignUsersVm
            {
                Id = appId
            });
        }


        public ActionResult SendToDepts(int appId)
        {
            return PartialView(new AssignDeptsVm
            {
                Id = appId
            });
        }

        public ActionResult Details(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var model = _unitOfWork.ApplicationRepo.FindWithNavProps(a => a.Id == id,
                _ => _.User,
                _=> _.Receivers,
                _=> _.Receivers.Select(c => c.Receiver),
                _=> _.Activities,
                _ => _.Activities.Select(a => a.User),
                _ => _.Signers,
                _ => _.Signers.Select(a => a.User),   
                _ => _.ApplicationType,
                _ => _.Attachment,
                _ => _.Approvals,
                _ => _.Approvals.Select(a => a.User)
                ).SingleOrDefault();

            if (model == null)
            {
                return HttpNotFound();
            }

            if (model.Receivers.FirstOrDefault()?.Received == true)
            {
                ViewBag.AssignUser = true;
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUsers(AssignUsersVm model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("AssignUsers", model);
            }

            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var assigneeIds =  model.UserIds.Select(int.Parse).ToList();
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.Id, _ => _.Signers, _ => _.Approvals, _=>_.Receivers).SingleOrDefault();
            if (application == null)
            {
                return HttpNotFound();
            }

            assigneeIds.ForEach(
                id => application.Signers.Add(new Signer
                {
                    UserId = id,
                    InviteTime = DateTime.Now

                }));
            var receivedApp = application.Receivers.SingleOrDefault(r => r.ReceiverId == user?.DepartmentId);

            if (receivedApp!=null && !receivedApp.Forwarded)
            {
                application.Approvals.Add(new Approval
                {
                    ApplicationId = model.Id,
                    InviteDate = DateTime.Now,
                    UserId = assigneeIds.First()
                });
            }

            _unitOfWork.Save();
            return Json(new{done=true});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignDepts(AssignDeptsVm model)
        {
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.Id,  _ => _.Receivers).SingleOrDefault();
            if (application == null)
            {
                return HttpNotFound();
            }

            if (model.DepartmentId != null)
                application.Receivers.Add(new ApplicationReceiver
                {
                    ReceiverId = model.DepartmentId.Value,
                    TimeSent = DateTime.Now,
                    Forwarded = true
                });
            _unitOfWork.Save();

            return Json(new {done = true});
        }
    }
}