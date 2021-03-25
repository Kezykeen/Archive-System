using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUpsertFile _upsertFile;
        private readonly IRoleService _roleService;
        private readonly IEmailSender _emailSender;

        public ApplicationsController(IUnitOfWork unitOfWork, IUpsertFile upsertFile, IRoleService roleService, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _upsertFile = upsertFile;
            _roleService = roleService;
            _emailSender = emailSender;
        }

        // GET: Applications
        [Authorize(Roles = "Student,Alumni,Staff")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Secretary")]
        public ActionResult Incoming()
        {
            return View();
        }

        [Authorize(Roles = "Secretary")]
        public ActionResult Received()
        {
            return View();
        }

        [Authorize(Roles = "Secretary")]
        public ActionResult Rejected()
        {
            return View();
        }

        [Authorize(Roles = "Secretary")]
        public ActionResult IncomingForwarded()
        {
            return View();
        }

        [Authorize(Roles = "Secretary")]
        public ActionResult RejectedForwarded()
        {
            return View();
        }

        [Authorize(Roles = "Secretary")]
        public ActionResult ReceivedForwarded()
        {
            return View();
        }

        [Authorize(Roles = "Secretary")]
        public ActionResult Archived()
        {
            return View();
        }

        [Authorize(Roles = "DeptOfficer")]
        public ActionResult ToSign()
        {
            return View();
        }

        [Authorize(Roles = "DeptOfficer")]
        public ActionResult Signed()
        {
            return View();
        }

        [Authorize(Roles = "DeptOfficer")]
        public ActionResult Declined()
        {
            return View();
        }

        [Authorize(Roles = "HOD")]
        public ActionResult ToApprove()
        {
            return View();
        }


        [Authorize(Roles = "HOD")]
        public ActionResult Approved()
        {
            return View();
        }

        [Authorize(Roles = "HOD")]
        public ActionResult Disapproved()
        {
            return View();
        }

        [Authorize(Roles = "HOD")]
        public ActionResult Forwarded()
        {
            return View();
        }


        [Authorize(Roles = "Student,Alumni,Staff")]
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
                model.ApplicationTypes = _unitOfWork.TicketRepo.Find(t => t.Designation == Designation.Staff);
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
            application.Activities.Add(
                new Activity
                {
                    UserId = user.Id,
                    Action = "Submitted An Application",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
                );
            _unitOfWork.ApplicationRepo.Add(application);
            _unitOfWork.Save();

            return Json(new { saved = true });

        }

        [Authorize(Roles = "Secretary")]
        public async  Task<ActionResult> Accept(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo.FindWithNavProps(a => a.Id == id, _=>_.User,  _=> _.Receivers, _=> _.Activities).SingleOrDefault();
            if (application == null) return HttpNotFound();
            var appReceived = application.Receivers.SingleOrDefault(r => r.ReceiverId == user?.DepartmentId);
            if (appReceived == null || appReceived.Received != null) return RedirectToAction("Details", new {id});
            appReceived.Received = true;
            appReceived.TimeReceived = DateTime.Now;
            application.Status = ApplicationStatus.Opened;

            application.Activities.Add(
                new Activity
                {
                    UserId = user.Id,
                    Action = "Accepted The Application",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            ); 
            application.UpdatedAt = DateTime.Now;
            try
            {
                await _unitOfWork.SaveAsync();
                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} has been Accepted by {user.Name}");
            }
            catch (Exception e)
            {
                TempData["Msg"] = $"{e.Message} \r\n{e.InnerException}";
            
            }
          
            return  RedirectToAction("Details", new{id});
        }

        [Authorize(Roles = "DeptOfficer")]

        public ActionResult Sign(int id)
        {
            ViewBag.Action = "Sign";
            return PartialView(new SignVm {AppId = id});
        }

        [Authorize(Roles = "HOD")]
        public ActionResult Disapprove(int id)
        {
            ViewBag.Action = "Disapprove";
            return PartialView("Sign",new SignVm { AppId = id });
        }

        [Authorize(Roles = "HOD")]
        public ActionResult SignApprove(int id)
        {
            ViewBag.Action = "SignApprove";
            return PartialView("Sign", new SignVm { AppId = id });
        }

        [Authorize(Roles = "HOD")]
        public ActionResult SignForward(int id)
        {
            ViewBag.Action = "SignForward";
            return PartialView("Sign", new SignVm { AppId = id });
        }

        [Authorize(Roles = "DeptOfficer")]
        public ActionResult Decline (int id)
        {
            ViewBag.Action = "Decline";
            return PartialView("Sign",new SignVm { AppId = id });
        }


        [Authorize(Roles = "Secretary")]
        public ActionResult Reject(int id)
        {
            ViewBag.Action = "Reject";
            return PartialView("Sign", new SignVm { AppId = id });
        }


        [Authorize(Roles = "Secretary")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reject(SignVm model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Action = "Reject";
                return PartialView("Sign",model);
            }
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo.FindWithNavProps(a => a.Id == model.AppId, _ => _.Receivers, _=> _.Activities).SingleOrDefault();
            if (application == null) return HttpNotFound();
            var appReceived = application.Receivers.SingleOrDefault(r => r.ReceiverId == user?.DepartmentId);
            if (appReceived == null || appReceived.Received != null) return RedirectToAction("Details", new { id =model.AppId});
            appReceived.Received = false;
            appReceived.TimeRejected = DateTime.Now;
            appReceived.RejectionMsg = model.Remark;
            application.Status = ApplicationStatus.Rejected;
            application.Activities.Add(
                new Activity
                {
                    UserId = user.Id,
                    Action = $@"Rejected The Application.
                                   Remark:{appReceived.RejectionMsg}",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );
            application.UpdatedAt = DateTime.Now;
            try
            {
                await _unitOfWork.SaveAsync();
                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} was rejected  by {user.Name}. Reason: {appReceived.RejectionMsg}");
                return Json(new { done = true });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                ViewBag.Action = "Reject";
                return PartialView("Sign", model);
            }
           


        }


        [Authorize(Roles = "DeptOfficer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sign(SignVm model)
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
                    _ => _.Approvals, _ => _.Signers, _=> _.Activities)
                .SingleOrDefault();
            if (application != null)
            {
                var callbackUrl = Url.Action("Details", "Applications", new { id = application.Id });
                var appToSign = application?.Approvals.SingleOrDefault(a => a.UserId == user?.Id);
                if (appToSign!=null )
                {
                    appToSign.Remark = model.Remark;
                    appToSign.Approve = true;
                    appToSign.Date = DateTime.Now;

                    application.Activities.Add(
                        new Activity
                        {
                            UserId = user.Id,
                            Action = "Signed the Application",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        }
                    );
                 
                    var signers = application.Signers.ToList();
                    if (signers.Last().UserId == user?.Id)
                    {
                        application.SendToHead = true;
                        application.Activities.Add(
                            new Activity
                            {
                                UserId = user.Id,
                                Action = "Sent Application To HOD",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            }
                        );
                   
                        application.UpdatedAt = DateTime.Now;
                        try
                        {
                            await _unitOfWork.SaveAsync();
                            await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                                $"Hi, {application.User.Name} Your  <a href=\"" + callbackUrl + "\">Application</a> is With the HOD for Approval");
                            return Json(new { done = true });

                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                            ViewBag.Action = "Sign";
                            return PartialView("Sign", model);
                        }
                      
                    }
                    signers = application.Signers.Where(s => s.UserId != user?.Id).OrderBy(s => s.InviteTime).ToList();
                    application.Approvals.Add(
                        new Approval
                        {
                            ApplicationId = model.AppId,
                            InviteDate = DateTime.Now,
                            UserId = signers.First().UserId
                        });
                    application.UpdatedAt = DateTime.Now;
                    await _unitOfWork.SaveAsync();
                    var nextSignerId = application.Approvals.Last().UserId;
                    var nextSigner = _unitOfWork.UserRepo.Get(nextSignerId);
                    try
                    {
                        await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                            $"Hi, {application.User.Name} {user.Name} Signed Your <a href=\"" + callbackUrl + "\">Application</a>");
                        await _emailSender.SendEmailAsync(nextSigner.Email, $"RE: {application.Title}",
                            $"Hi {nextSigner.Name}, You have Been Invited to Sign This  <a href=\"" + callbackUrl + "\">Application</a>");
                        return Json(new { done = true });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                        ViewBag.Action = "Sign";
                        return PartialView("Sign", model);
                    }
                   
                   
                }
            }

            ViewBag.Action = "Sign";
            return PartialView(model);
        }


        [Authorize(Roles = "DeptOfficer")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Decline(SignVm model)
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
                    _ => _.Approvals, _ => _.Activities)
                .SingleOrDefault();
            var appToSign = application?.Approvals.SingleOrDefault(a => a.UserId == user?.Id);
            if (appToSign != null)
            {
                appToSign.Remark = model.Remark;
                appToSign.Approve = false;
                appToSign.Date = DateTime.Now;
                application.Activities.Add(
                    new Activity
                    {
                        UserId = user.Id,
                        Action = $"Declined the Application.\r\nRemark:{model.Remark}",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                var callbackUrl = Url.Action("Details", "Applications", new { id = application.Id });
                application.UpdatedAt = DateTime.Now;

                try
                {
                    await _unitOfWork.SaveAsync();
                    await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                        $"Hi, {application.User.Name} {user.Name} Declined Your <a href=\"" + callbackUrl + $"\">Application</a>.\r\n Reason:{model.Remark}");

                    return Json(new { done = true });
                }
                catch (Exception e)
                {
                  
                    ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                    ViewBag.Action = "Decline";
                    return PartialView("Sign", model);
                }
              
            }
            ViewBag.Action = "Decline";
            return PartialView("Sign",model);
        }


        [Authorize(Roles = "HOD")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignApprove(SignVm model)
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
                    _ => _.Approvals, _=> _.Activities)
                .SingleOrDefault();
            if (application != null)
            {
                var callbackUrl = Url.Action("Details", "Applications", new { id = application.Id });
                application.Approvals.Add(new Approval
                {
                    ApplicationId = application.Id,
                    InviteDate = DateTime.Now,
                    Approve = true,
                    Date = DateTime.Now,
                    UserId = user.Id,
                    Remark = model.Remark
                });
                application.Approve = true;
                application.Activities.Add(
                    new Activity
                    {
                        UserId = user.Id,
                        Action = "Approved the Application",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                application.UpdatedAt = DateTime.Now;

                try
                {
                    await _unitOfWork.SaveAsync();
                    await _emailSender.SendEmailAsync(application.User.Email, $"Approved: {application.Title}",
                        $"Hi, {application.User.Name} The Hod Signed and Approved Your <a href=\"" + callbackUrl + "\">Application</a>");
                    return Json(new { done = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                    ViewBag.Action = "SignApprove";
                    return PartialView("Sign", model);
                }
                
            }

            ViewBag.Action = "SignApprove";
            return PartialView("Sign", model);
        }

        [Authorize(Roles = "HOD")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignForward(SignVm model)
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
                    _ => _.Approvals, _=> _.Activities)
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
                application.Activities.Add(
                    new Activity
                    {
                        UserId = user.Id,
                        Action = "Signed the Application",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                application.UpdatedAt = DateTime.Now;
                var callbackUrl = Url.Action("Details", "Applications", new { id = application.Id });
                try
                {
                    await _unitOfWork.SaveAsync();
                    await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                        $"Hi, {application.User.Name} The Hod Signed  Your <a href=\"" + callbackUrl + "\">Application</a> and will be Forwarded to another Department");
                    return Json(new { done = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                    ViewBag.Action = "SignForward";
                    return PartialView("Sign", model);
                }
                

            }

            ViewBag.Action = "SignForward";
            return PartialView("Sign", model);
        }


        [Authorize(Roles = "HOD")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disapprove(SignVm model)
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
                    _ => _.Approvals, _ => _.Activities)
                .SingleOrDefault();
            if (application != null)
            {
                application.Approvals.Add(new Approval
                {
                    ApplicationId = application.Id,
                    InviteDate = DateTime.Now,
                    Approve = false,
                    Date = DateTime.Now,
                    UserId = user.Id,
                    Remark = model.Remark,
                });
                application.Approve = false;
                application.Activities.Add(
                    new Activity
                    {
                        UserId = user.Id,
                        Action = $"Disapproved the Application.\r\nRemark:{model.Remark}",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                application.UpdatedAt = DateTime.Now;
                var callbackUrl = Url.Action("Details", "Applications", new { id = application.Id });
                try
                {
                    await _unitOfWork.SaveAsync();
                    await _emailSender.SendEmailAsync(application.User.Email, $"Disapproved: {application.Title}",
                        $"Hi, {application.User.Name} The Hod Disapproved  Your <a href=\"" + callbackUrl + $"\">Application</a>. /r/n Reason: {model.Remark} ");
                    return Json(new { done = true });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                    ViewBag.Action = "Disapprove";
                    return PartialView("Sign", model);
                }
               
            }

            ViewBag.Action = "Disapprove";
            return PartialView("Sign", model);
        }
        public ActionResult AssignUsers(int appId)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            if (user==null)
            {
                return HttpNotFound();
            }
            return PartialView(new AssignUsersVm
            {
                Id = appId
            });
        }

        [Authorize(Roles = "HOD")]
        public ActionResult SendToDepts(int appId)
        {
            return PartialView(new AssignDeptsVm
            {
                Id = appId
            });
        }

        [Authorize(Roles = "Secretary, Secretary, HOD, DeptOfficer,Alumni,Staff")]
        public ActionResult Details(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo.FindWithNavProps(a => a.Id == id,
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
           
            if (application == null)
            {
                return HttpNotFound();
            }
            var secretary = application.Receivers.Select(r => r.ReceiverId).Contains(user.DepartmentId) && User.IsInRole("Secretary");
            var hod = application.SendToHead && application.Receivers.Select(a => a.ReceiverId).Contains(user.DepartmentId) &&
                      User.IsInRole("HOD");
            var deptOfficer = application.Approvals.Select(a => a.UserId).Contains(user.Id) &&
                              User.IsInRole("DeptOfficer") && application.Archive == false;
            var appOwner = application.UserId == user.Id && application.Archive == false;
            if (!secretary && !(deptOfficer | hod) && !appOwner) return HttpNotFound();
            if (application.User.UserId == User.Identity.GetUserId() &&
                application.Status == ApplicationStatus.Rejected)
            {
                ViewBag.AddNewVersion = true;
            }

            if (User.IsInRole("Secretary"))
            {
                ViewBag.Secretary = true;
                if (application.Receivers.FirstOrDefault()?.Received == true 
                    && secretary && application.Archive == false && application.Approve == null)
                {
                    ViewBag.AssignUser = true;
                }
            }

            if (deptOfficer && application.Approve == null)
            {
                ViewBag.DeptOfficer = true;
                
            }

            if (!User.IsInRole("HOD")) return View(application);
            {
                ViewBag.HOD = true;
                if (application.Approvals.Select(a => a.UserId).Contains(user.Id) && application.Approve == null)
                {
                    ViewBag.Forward = true;
                }
            }



            return View(application);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<ActionResult> AssignUsers(AssignUsersVm model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("AssignUsers", model);
            }

            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var assigneeIds =  model.UserIds.Select(int.Parse).ToList();
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.Id && a.Approve == null && a.Archive==false, 
                    _ => _.Signers, _ => _.Approvals, _=>_.Receivers, _=>_.Activities).SingleOrDefault();
            
            if (application == null)
            {
                ModelState.AddModelError("", @"Invalid Operation!");
                return PartialView("AssignUsers", model);
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
                application.Status = ApplicationStatus.InProgress;
                application.UpdatedAt = DateTime.Now;
                var nextSignerId = application.Approvals.First().UserId;
                var nextSigner = _unitOfWork.UserRepo.Get(nextSignerId);
                application.Activities.Add(
                    new Activity
                    {
                        UserId = user.Id,
                        Action = $"Assigned the Application to {nextSigner.Name}",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                application.UpdatedAt = DateTime.Now;
                var callbackUrl = Url.Action("Details", "Applications", new { id = application.Id });

                try
                {
                    await _unitOfWork.SaveAsync();
                    await _emailSender.SendEmailAsync(nextSigner.Email, $"RE: {application.Title}",
                        $"Hi {nextSigner.Name}, You have Been Invited to Sign This  <a href=\"" + callbackUrl + "\">Application</a>");
                    await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                        $"Hi, {application.User.Name} Your <a href=\"" + callbackUrl + $"\">Application</a> has been Assigned to {nextSigner.Email}");
                    Response.StatusCode = 201;
                    return PartialView("_Assignees", application);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                    return PartialView("AssignUsers", model);
                }

            }
            ModelState.AddModelError("", "Invalid Operation!");
            return PartialView("AssignUsers", model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignDepts(AssignDeptsVm model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("SendToDepts", model);
            }
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Id == model.Id && a.Approve == null
                                                        && a.Archive == false,  _ => _.Receivers, _=> _.Activities).SingleOrDefault();
            if (application == null)
            {
                ModelState.AddModelError("", "Invalid Operation");
                return PartialView("SendToDepts", model);
            }

            if (model.DepartmentId != null)
                application.Receivers.Add(new ApplicationReceiver
                {
                    ReceiverId = model.DepartmentId.Value,
                    TimeSent = DateTime.Now,
                    Forwarded = true
                });
            application.SendToHead = false;
            application.UpdatedAt = DateTime.Now;
            _unitOfWork.Save();
            var dept = _unitOfWork.DeptRepo.Get(model.DepartmentId.Value);
            application.Activities.Add(
                new Activity
                {
                    UserId = user.Id,
                    Action = $"Forwarded the Application to {dept.Name}",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );
            application.UpdatedAt = DateTime.Now;
            var callbackUrl = Url.Action("Details", "Applications", new { id = application.Id });
            try
            {
                await _unitOfWork.SaveAsync();
                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} The Hod Forwarded  Your <a href=\"" + callbackUrl + $"\">Application</a> to {dept.Name}");
                Response.StatusCode = 201;
                return PartialView("_FwdDepts", application.Receivers);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", $"{e.Message} \r\n{e.InnerException}");
                return PartialView("SendToDepts", model);
              
            }
           
        }

        public async Task<ActionResult>  Archive(int id)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            var application = _unitOfWork.ApplicationRepo.FindWithNavProps(a => a.Id == id, _ => _.Receivers, _ => _.Activities).SingleOrDefault();
            if (application == null) return HttpNotFound();
            var appReceived = application.Receivers.SingleOrDefault(r => r.ReceiverId == user?.DepartmentId);
            if (appReceived == null || appReceived.Received != null) return RedirectToAction("Details", new { id });
            if (application.Status == ApplicationStatus.Closed)
            {
                application.Archive = true;
                application.Activities.Add(
                    new Activity
                    {
                        UserId = user.Id,
                        Action = "Archived the Application",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                application.UpdatedAt = DateTime.Now;
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Archived");
            }
           
            TempData["Msg"] = "This Document Is not Closed!, Kindly Set it As Close!";

            return RedirectToAction("Details", new { id });
        }

        public FileContentResult GetFile(int id, string fileName)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(u => u.UserId == currentUserId).SingleOrDefault();
            if (user == null)
            {
                return null;
            }
            var application = _unitOfWork.ApplicationRepo.FindWithNavProps(a => a.Id == id,
                _ => _.User,
                _ => _.Receivers,
                _ => _.Receivers.Select(c => c.Receiver),
                _ => _.Attachment,
                _ => _.Approvals,
                _ => _.Approvals.Select(a => a.User)
            ).SingleOrDefault();
            if (application == null)
            {
                return null;
            }
            var secretary = application.Receivers.Select(r => r.ReceiverId).Contains(user.DepartmentId) && User.IsInRole("Secretary");
            var hod = application.SendToHead && application.Receivers.Select(a => a.ReceiverId).Contains(user.DepartmentId) &&
                      User.IsInRole("HOD");
            var deptOfficer = application.Approvals.Select(a => a.UserId).Contains(user.Id) &&
                              User.IsInRole("DeptOfficer") && application.Archive == false;
            var fileOwner = application.UserId == user.Id && application.Archive == false;

            if (!secretary && !hod && !deptOfficer && !fileOwner) return null;
            var file = application.Attachment;
            if (file == null)
            {
                return null;
            }
            Response.AddHeader("Content-Disposition", "inline; filename=" + fileName);
            return File(file.Content, file.ContentType);

        }

    }
}