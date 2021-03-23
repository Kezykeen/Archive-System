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
        private readonly IRoleService _roleService;

        public ApplicationsController(IUnitOfWork unitOfWork, IUpsertFile upsertFile, IRoleService roleService)
        {
            _unitOfWork = unitOfWork;
            _upsertFile = upsertFile;
            _roleService = roleService;
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
            appReceived.TimeReceived = DateTime.Now;
            application.Status = ApplicationStatus.Opened;
            _unitOfWork.Save();
            return  RedirectToAction("Details", new{id});

          
        }

        public ActionResult Sign(int id)
        {
            ViewBag.Action = "Sign";
            return PartialView(new SignVm {AppId = id});
        }

        public ActionResult Disapprove(int id)
        {
            ViewBag.Action = "Disapprove";
            return PartialView("Sign",new SignVm { AppId = id });
        }
        public ActionResult SignApprove(int id)
        {
            ViewBag.Action = "SignApprove";
            return PartialView("Sign", new SignVm { AppId = id });
        }
        public ActionResult SignForward(int id)
        {
            ViewBag.Action = "SignForward";
            return PartialView("Sign", new SignVm { AppId = id });
        }

        public ActionResult Decline (int id)
        {
            ViewBag.Action = "Decline";
            return PartialView("Sign",new SignVm { AppId = id });
        }

        public ActionResult Reject(int id)
        {
            ViewBag.Action = "Reject";
            return PartialView("Sign", new SignVm { AppId = id });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(SignVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = "Reject";
                return PartialView("Sign",model);
            }
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo.FindWithNavProps(a => a.Id == model.AppId, _ => _.Receivers).SingleOrDefault();
            if (application == null) return HttpNotFound();
            var appReceived = application.Receivers.SingleOrDefault(r => r.ReceiverId == user?.DepartmentId);
            if (appReceived == null || appReceived.Received != null) return RedirectToAction("Details", new { id =model.AppId});
            appReceived.Received = false;
            appReceived.TimeRejected = DateTime.Now;
            appReceived.RejectionMsg = model.Remark;
            application.Status = ApplicationStatus.Rejected;
            _unitOfWork.Save();
            return RedirectToAction("Incoming");


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sign(SignVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = "Sign";
                return PartialView(model);
            }
            
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.AppId,
                    _ => _.Approvals, _ => _.Signers)
                .SingleOrDefault();
            var appToSign = application?.Approvals.SingleOrDefault(a => a.UserId == user?.Id);
            if (appToSign!=null )
            {
                appToSign.Remark = model.Remark;
                appToSign.Approve = true;
                appToSign.Date = DateTime.Now;

                var signers = application.Signers.ToList();
                if (signers.Last().UserId == user?.Id)
                {
                    application.SendToHead = true;

                    _unitOfWork.Save();
                    return Json(new { head = true });

                }
                signers = application.Signers.Where(s => s.UserId != user?.Id).OrderBy(s => s.InviteTime).ToList();
                application.Approvals.Add(
                    new Approval
                    {
                        ApplicationId = model.AppId,
                        InviteDate = DateTime.Now,
                        UserId = signers.First().UserId
                    });
                _unitOfWork.Save();
                return Json(new {done = true});
            }
            ViewBag.Action = "Sign";
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Decline(SignVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = "Decline";
                return PartialView("Sign",model);
            }

            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.AppId,
                    _ => _.Approvals)
                .SingleOrDefault();
            var appToSign = application?.Approvals.SingleOrDefault(a => a.UserId == user?.Id);
            if (appToSign != null)
            {
                appToSign.Remark = model.Remark;
                appToSign.Approve = false;
                appToSign.Date = DateTime.Now;

                _unitOfWork.Save();
                return Json(new { done = true });
            }
            ViewBag.Action = "Decline";
            return PartialView("Sign",model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignApprove(SignVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = "SignApprove";
                return PartialView("Sign", model);
            }

            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.AppId && a.SendToHead,
                    _ => _.Approvals)
                .SingleOrDefault();
            if (application != null)
            {
                application.Approvals.Add(new Approval
                {
                    ApplicationId = application.Id,
                    InviteDate = DateTime.Now,
                    Approve = true,
                    Date = DateTime.Now,
                    UserId = user.Id,
                    Remark = model.Remark,
                });
                application.Approve = true;
                _unitOfWork.Save();
                return Json(new { done = true });

            }

            ViewBag.Action = "SignApprove";
            return PartialView("Sign", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignForward(SignVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = "SignForward";
                return PartialView("Sign", model);
            }

            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.AppId && a.SendToHead,
                    _ => _.Approvals)
                .SingleOrDefault();
            if (application != null)
            {
                application.Approvals.Add(new Approval
                {
                    ApplicationId = application.Id,
                    InviteDate = DateTime.Now,
                    Approve = true,
                    Date = DateTime.Now,
                    UserId = user.Id,
                    Remark = model.Remark,
                });
                _unitOfWork.Save();
                return Json(new { done = true });

            }

            ViewBag.Action = "SignForward";
            return PartialView("Sign", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disapprove(SignVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = "Disapprove";
                return PartialView("Sign", model);
            }

            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.AppId,
                    _ => _.Approvals)
                .SingleOrDefault();
            if (application != null)
            {
                application.Approve = false;
                _unitOfWork.Save();
                return Json(new { done = true });
            }

            ViewBag.Action = "Disapprove";
            return PartialView("Sign", model);
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
            application.SendToHead = false;
            _unitOfWork.Save();

            return Json(new {done = true});
        }
    }
}