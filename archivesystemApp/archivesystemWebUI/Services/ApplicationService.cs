using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace archivesystemWebUI.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IApplicationRepo _applicationRepo;
        private readonly IDeptRepository _deptRepository;
        private readonly ITicketRepo _ticketRepo;
        private readonly IUpsertFile _upsertFile;
        private readonly IEmailSender _emailSender;
        private static HttpContext Context => HttpContext.Current;
        private readonly UrlHelper _url = new UrlHelper(Context.Request.RequestContext);
        private static IIdentity User => Context.User.Identity;
        private static string UserId => User.GetUserId();

        public ApplicationService(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository, 
            IApplicationRepo applicationRepo,
            IDeptRepository repository,
            ITicketRepo ticketRepo,
            IUpsertFile upsertFile,
            IRoleService roleService,
            IEmailSender emailSender
            )
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _applicationRepo = applicationRepo;
            _deptRepository = repository;
            _ticketRepo = ticketRepo;
            _upsertFile = upsertFile;
            _emailSender = emailSender;
        }




        public IEnumerable<Ticket> GetApplicationTypes(Designation designation)
        {
            switch (designation)
            {
                case Designation.Alumni:
                    return _ticketRepo.Find(t => t.Designation == Designation.Alumni);
                case Designation.Staff:
                    return _ticketRepo.Find(t => t.Designation == Designation.Staff);
                case Designation.Student:
                    return _ticketRepo.Find(t => t.Designation == Designation.Student);
                default:
                    return null;
            }
        }


        public bool Create(ApplicationVm model, HttpPostedFileBase fileBase, AppUser user)
        {
            if (user == null)
            {
                return false;
            }
            
            if (fileBase != null)
            {
                model.Attachment = _upsertFile.Save(model.Attachment, fileBase);
            }

            var application = Mapper.Map<Application>(model);

            application.Title =
                $"{user.TagId}_{_ticketRepo.Get(model.ApplicationTypeId).Acronym}_{DateTime.Now:yy/MM/dd}";

            application.Receivers.Add(
               Recipient(model.DepartmentId)
            );

            application.Activities.Add(
                    ActivityLog(user.Id, "Submitted An Application")
                );

            application.UpdatedAt = DateTime.Now;

            _applicationRepo.Add(application);
            _unitOfWork.Save();

            return true;
        }

        public async Task<(bool reject, string msg)> Reject(SignVm model)
        {
           
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();

            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == model.AppId, _ => _.Receivers, _ => _.Activities).SingleOrDefault();

            if (application == null || user == null) return (false, "An Error Occured!");

            var appReceived = application.Receivers.SingleOrDefault(r => r.ReceiverId == user.DepartmentId);

            if (appReceived == null || appReceived.Received != null) return (false, "Bad Request!");

            appReceived.Received = false;
            appReceived.TimeRejected = DateTime.Now;
            appReceived.RejectionMsg = model.Remark;
            application.Status = ApplicationStatus.Rejected;

            application.Activities.Add(
                ActivityLog(user.Id, $@"Rejected The Application.
                   Remark:{appReceived.RejectionMsg}"));

            application.UpdatedAt = DateTime.Now;

            try
            {
                await _unitOfWork.SaveAsync();

                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} was rejected  by {user.Name}. Reason: {appReceived.RejectionMsg}");

                return (true, "");
            }
            catch (Exception e)
            {
                return (false, $"{e.Message} \r\n{e.InnerException}");
            }
        }

        public async Task<(bool accept, string msg)> Accept(int id)
        {
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();
            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == id,
                    _ => _.User, _ => _.Receivers, _ => _.Activities)
                .SingleOrDefault();
            if (application == null || user == null) return (false, "An Error Occurred!");
            var appReceived = application.Receivers.SingleOrDefault(r => r.ReceiverId == user.DepartmentId);
            if (appReceived == null || appReceived.Received != null) return (false, "An Error Occurred");

            appReceived.Received = true;
            appReceived.TimeReceived = DateTime.Now;
            application.Status = ApplicationStatus.Opened;
            application.Activities.Add(
                ActivityLog(user.Id, "Accepted The Application"));

            application.UpdatedAt = DateTime.Now;

            try
            {
                var callbackUrl = Task.Run(() =>
                {
                    _url.Action("Details", "Applications", new { id = application.Id });
                });

                await callbackUrl;
                await _unitOfWork.SaveAsync();
                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} Your <a href=\"" + callbackUrl + "\">Application</a> has been Accepted by {user.Name}");
                return (true, "");
            }
            catch (Exception e)
            {
                return (false, $"{e.Message} \r\n{e.InnerException}");

            }
        }

        public async Task<(bool sign, string msg)> Sign(SignVm model)
        {
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();
            if (user == null) return (false, "An Error Occured!");

            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == model.AppId, 
                    _ => _.Approvals,
                    _ => _.Signers,
                    _ => _.Activities)
                .SingleOrDefault();

            var appToSign = application?.Approvals.SingleOrDefault(a => a.UserId == user.Id);

            if (appToSign == null) return (false, "An Error Occured!");

            UpdateApproval(appToSign, model.Remark, true);

            application.Activities.Add(
                ActivityLog(user.Id, "Signed the Application")
            );

            var signers = application.Signers.ToList();

            if (signers.Last().UserId == user.Id)
            {
                var (sent, msg) = await SendToHod(application, user);

                return sent ? (true, msg) : (false, msg);
            }

            signers = application.Signers.Where(s => s.UserId != user.Id)
                .OrderBy(s => s.InviteTime).ToList();

            application.Approvals.Add(NewApproval(model.AppId, signers.First().UserId));

            application.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveAsync();

            var nextSignerId = application.Approvals.Last().UserId;
            var nextSigner = _userRepository.Get(nextSignerId);

            try
            {
                var callbackUrl = Task.Run(() =>
                {
                    _url.Action("Details", "Applications", new { id = application.Id });
                });

                await callbackUrl;

                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} {user.Name} Signed Your <a href=\"" + callbackUrl + "\">Application</a>");

                await _emailSender.SendEmailAsync(nextSigner.Email, $"RE: {application.Title}",
                    $"Hi {nextSigner.Name}, You have Been Invited to Sign This  <a href=\"" + callbackUrl + "\">Application</a>");

                return (true, "");
            }
            catch (Exception e)
            {
                return (false, $"{e.Message} \r\n{e.InnerException}");
            }
        }



        public async Task<(bool decline, string msg)> Decline(SignVm model)
        {
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();

            if (user == null)
            {
                return (false, "An Error Occured!");
            }

            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == model.AppId,
                    _ => _.Approvals, _ => _.Activities)
                .SingleOrDefault();

            var appToSign = application?.Approvals.SingleOrDefault(a => a.UserId == user.Id);

            if (appToSign == null) return (true, "An Error Occured!");
            UpdateApproval(appToSign, model.Remark, false);
               
            application.Activities.Add(
                ActivityLog(user.Id, $"Declined the Application.\r\nRemark:{model.Remark}")
                   
            );
            application.UpdatedAt = DateTime.Now;

            try
            {
                await _unitOfWork.SaveAsync();
                var callbackUrl = Task.Run(() => { _url.Action("Details", "Applications", new { id = application.Id }); });
                await callbackUrl;
                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} {user.Name} Declined Your <a href=\"" + callbackUrl + $"\">Application</a>.\r\n Reason:{model.Remark}");

                return (true, "");
            }
            catch (Exception e)
            {
                return (true, $"{e.Message} \r\n{e.InnerException}");
            }
        }



        public async Task<(bool sent, string msg )> SendToHod(Application application, AppUser user)
        {
            application.SendToHead = true;

            application.Activities.Add(
                ActivityLog(user.Id, "Sent Application To HOD")
            );

            application.UpdatedAt = DateTime.Now;
            try
            {
                await _unitOfWork.SaveAsync();

                var callbackUrl = Task.Run(() =>
                {
                    _url.Action("Details", "Applications", new { id = application.Id });
                });
                await callbackUrl;
                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} Your  <a href=\"" + callbackUrl + "\">Application</a> is With the HOD for Approval");
                return (true, "");
            }
            catch (Exception e)
            {
                return (false, $"{e.Message} \r\n{e.InnerException}");
            }
        }

        public async Task<(bool approve, string msg)> SignApprove(SignVm model)
        {
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();
            if (user == null)
            {
                return (false, "An Error Occured!");
            }

            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == model.AppId && a.SendToHead,
                    _ => _.Approvals, _ => _.Activities)
                .SingleOrDefault();

            if (application == null) return (false, "An Error Occurred!");

            var hodSign = NewApproval(application.Id, user.Id);
            hodSign.Approve = true;
            hodSign.Remark = model.Remark;
            hodSign.Date = DateTime.Now;
                
            application.Approvals.Add(hodSign);
            application.Approve = true;
            application.Activities
                .Add(ActivityLog(user.Id, "Approved the Application"));

            application.UpdatedAt = DateTime.Now;

            try
            {
                var callbackUrl = Task.Run(() =>
                {
                    _url.Action("Details", "Applications", new {id = application.Id});
                });

                await callbackUrl;
                await _unitOfWork.SaveAsync();
                await _emailSender.SendEmailAsync(application.User.Email, $"Approved: {application.Title}",
                    $"Hi, {application.User.Name} The Hod Signed and Approved Your <a href=\"" + callbackUrl + "\">Application</a>");
                return (true, "");
            }
            catch (Exception e)
            {
                return (true, $"{e.Message} \r\n{e.InnerException}");
            }
           
        }

        public async Task<(bool forward, string msg)> SignForward(SignVm model)
        {
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();
            if (user == null)
            {
                return (false, "An Error Occured!");
            }

            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == model.AppId && a.SendToHead,
                    _ => _.Approvals, _ => _.Activities)
                .SingleOrDefault();

            if (application == null) return (false, "AN Error Occured!");

            var hodSign = NewApproval(application.Id, user.Id);
            hodSign.Approve = true;
            hodSign.Remark = model.Remark;
            hodSign.Date = DateTime.Now;

            application.Approvals.Add(hodSign);

            application.Activities.Add(
                ActivityLog(user.Id, "Signed the Application"));

            application.UpdatedAt = DateTime.Now;

            try
            {
                var callbackUrl = Task.Run(() =>
                {
                    _url.Action("Details", "Applications", new { id = application.Id });
                });

                await callbackUrl;
                await _unitOfWork.SaveAsync();
                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} The Hod Signed  Your <a href=\"" + callbackUrl + "\">Application</a> and will be Forwarded to another Department");
                return (true, "");
            }
            catch (Exception e)
            {
                return (false, $"{e.Message} \r\n{e.InnerException}");
            }

        }

        public async Task<(bool disapprove, string msg)> Disapprove(SignVm model)
        {

            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();
            if (user == null)
            {
                return (false, "An Error Occured!");
            }

            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == model.AppId,
                    _ => _.Approvals, _ => _.Activities)
                .SingleOrDefault();

            if (application == null) return (false, "An Error Occured!");
            var hodSign = NewApproval(application.Id, user.Id);
            hodSign.Approve = false;
            hodSign.Remark = model.Remark;
            hodSign.Date = DateTime.Now;

            application.Approvals.Add(hodSign);

            application.Approve = false;

            application.Activities.Add(
                ActivityLog(user.Id, $"Disapproved the Application.\r\nRemark:{model.Remark}")
            );
            application.UpdatedAt = DateTime.Now;
               
            try
            {
                var callbackUrl = Task.Run(() =>
                {
                    _url.Action("Details", "Applications", new { id = application.Id });
                });

                await callbackUrl;
                await _unitOfWork.SaveAsync();
                await _emailSender.SendEmailAsync(application.User.Email,
                    $"Disapproved: {application.Title}",
                    $"Hi, {application.User.Name} The Hod Disapproved  Your <a href=\"" 
                    + callbackUrl + $"\">Application</a>. /r/n Reason: {model.Remark} ");

                return (true, "");
            }
            catch (Exception e)
            {
                return (false, $"{e.Message} \r\n{e.InnerException}");
            }

        }

       

        public ( bool found, Application result )GetApplication(int id)
        {
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();
            var application = _applicationRepo.FindWithNavProps(a => a.Id == id,
            _ => _.User,
            _ => _.Receivers,
            _ => _.Receivers.Select(c => c.Receiver),
            _ => _.Activities,
            _ => _.Activities.Select(a => a.User),
            _ => _.Signers,
            _ => _.Signers.Select(a => a.User),
            _ => _.ApplicationType,
            _ => _.Attachment,
            _ => _.Approvals,
            _ => _.Approvals.Select(a => a.User))
                .SingleOrDefault();

            if (user == null || application == null) return (false, null);
            return (true, application);
        }

        public (bool secetary, bool hod, bool deptOfficer, bool appOwner) DoCheck(Application application, AppUser user)
        {
            return (
                application.Receivers.Select(r => r.ReceiverId).Contains(user.DepartmentId) &&
                Context.User.IsInRole("Secretary"),
                application.SendToHead && application.Receivers.Select(a => a.ReceiverId).Contains(user.DepartmentId) &&
                Context.User.IsInRole("HOD"),
                application.Approvals.Select(a => a.UserId).Contains(user.Id) && Context.User.IsInRole("DeptOfficer") &&
                application.Archive == false,
                application.UserId == user.Id && application.Archive == false
                );
        }

        public async Task<(bool assign, string msg, Application application)> AssignUsers(AssignUsersVm model)
        {
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();


            var assigneeIds = model.UserIds.Select(int.Parse).ToList();
            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == model.Id && a.Approve == null && a.Archive == false,
                    _ => _.Signers, _ => _.Approvals, _ => _.Receivers, _ => _.Activities).SingleOrDefault();
            if (user == null || application == null) return (false, "An Error Occured!", null);

            foreach (var id in assigneeIds)
            {
                if (application.Signers.Select(a => a.UserId).Contains(id))
                {
                    var signer = _userRepository.Get(id);
                    return (false, $"{signer.Name} Already Exist", null);
                }

                application.Signers.Add(new Signer
                {
                    UserId = id,
                    InviteTime = DateTime.Now
                });
            }

            var receivedApp = application.Receivers.SingleOrDefault(r => r.ReceiverId == user.DepartmentId);

            if (receivedApp != null && !receivedApp.Forwarded)
            {
                application.Approvals.Add(
                    NewApproval(model.Id, assigneeIds.First()));

                application.Status = ApplicationStatus.InProgress;
                
                var nextSignerId = application.Approvals.First().UserId;
                var nextSigner = _userRepository.Get(nextSignerId);
                application.Activities.Add(
                    ActivityLog(user.Id, $"Assigned the Application to {nextSigner.Name}"));
                application.UpdatedAt = DateTime.Now;


                try
                {
                     var callbackUrl = Task.Run(() =>
                    {
                        _url.Action("Details", "Applications", new { id = application.Id });
                    });

                    await callbackUrl;
                    await _unitOfWork.SaveAsync();

                    await _emailSender.SendEmailAsync(nextSigner.Email, $"RE: {application.Title}",
                        $"Hi {nextSigner.Name}, You have Been Invited to Sign This  <a href=\"" + callbackUrl +
                        "\">Application</a>");

                    await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                        $"Hi, {application.User.Name} Your <a href=\"" + callbackUrl +
                        $"\">Application</a> has been Assigned to {nextSigner.Name}");

                    return (true, "", application);
                }
                catch (Exception e)
                {
                    return (false, $"{e.Message} \r\n{e.InnerException}", null);
                }

            }

            if (receivedApp == null || !receivedApp.Forwarded) return (false, "An Error Occured!", null);
            {
                foreach (var id in assigneeIds)
                {
                    if (application.Signers.Select(a => a.UserId).Contains(id))
                    {
                        var signer = _userRepository.Get(id);
                        return (false, $"{signer.Name} Already Exist", null);
                    }

                    application.Signers.Add(new Signer
                    {
                        UserId = id,
                        InviteTime = DateTime.Now
                    });
                }
                application.UpdatedAt = DateTime.Now;
                
                try
                {
                    var callbackUrl = Task.Run(() =>
                    {
                        _url.Action("Details", "Applications", new { id = application.Id });
                    });

                    await callbackUrl;
                    await _unitOfWork.SaveAsync();

                    return (true, "", application);
                }
                catch (Exception e)
                {
                    return (false, $"{e.Message} \r\n{e.InnerException}", null);
                }
            }
        }


        public async Task<(bool assign, string msg, Application application)> AssignToDept(AssignDeptsVm model)
        {

            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();

            var application = _applicationRepo
                .FindWithNavProps(a => a.Id == model.Id && a.Approve == null
                                                        && a.Archive == false, _ => _.Receivers, _ => _.Activities)
                .SingleOrDefault();
            if (user == null || application == null)
            {
                return (false, "An Error Occurred", null);
            }

            if (model.DepartmentId == null || application.Receivers.Select(a => a.ReceiverId).Contains(model.Id))
                return (false, "An Error Occured!", null);
            var recipient = Recipient(model.DepartmentId.Value);
            recipient.Forwarded = true;
            application.Receivers.Add(recipient);
            application.SendToHead = false;

            var dept = _deptRepository.Get(model.DepartmentId.Value);
            application.Activities.Add(
                ActivityLog(user.Id, $"Forwarded the Application to {dept.Name}"));
            application.UpdatedAt = DateTime.Now;

            try
            {
                var callbackUrl = Task.Run(() =>
                {
                    _url.Action("Details", "Applications", new { id = application.Id });
                });

                await callbackUrl;
                await _unitOfWork.SaveAsync();
                await _emailSender.SendEmailAsync(application.User.Email, $"RE: {application.Title}",
                    $"Hi, {application.User.Name} The Hod Forwarded  Your <a href=\"" + callbackUrl +
                    $"\">Application</a> to {dept.Name}");

                return (true, "", application);
            }
            catch (Exception e)
            {
                return (false, $"{e.Message} \r\n{e.InnerException}", null);

            }

        }


        public (bool archive, string msg) Archive(int id)
        {
            
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();
            var application = _applicationRepo.FindWithNavProps(a => a.Id == id, _ => _.Receivers, _ => _.Activities).SingleOrDefault();
            if (application == null || user == null) return (false, "An Error Occured!");
            var appReceived = application.Receivers.SingleOrDefault(r => r.ReceiverId == user.DepartmentId);
            if (appReceived == null || appReceived.Received != null) return (false, "An Error Occured!");
            if (application.Status != ApplicationStatus.Closed)
                return (false, "This Document Is not Closed!, Kindly Set it As Close!");
            application.Archive = true;
            application.Activities.Add(
                ActivityLog(user.Id, "Archived the Application")
            );
            application.UpdatedAt = DateTime.Now;
            _unitOfWork.Save();
            return (true, "");

        }

        public File GetFile(int id, string fileName)
        {
            var user = _userRepository.Find(u => u.UserId == UserId).SingleOrDefault();
            
            var application = _applicationRepo.FindWithNavProps(a => a.Id == id,
                _ => _.User,
                _ => _.Receivers,
                _ => _.Receivers.Select(c => c.Receiver),
                _ => _.Attachment,
                _ => _.Approvals,
                _ => _.Approvals.Select(a => a.User)
            ).SingleOrDefault();

            if (application == null || user == null)
            {
                return null;
            }

            var (secretary, hod, deptOfficer, fileOwner) = DoCheck(application, user);

            if (!secretary && !hod && !deptOfficer && !fileOwner) return null;
            var file = application.Attachment;

            return file;
        }

        public IEnumerable<Application> UserApplications()
        {
            var user = _userRepository.Find(c => c.UserId == UserId).SingleOrDefault();
            if (user == null)
            {
                return null;
            }
            var applications = _applicationRepo
                .FindWithNavProps(a => a.UserId == user.Id,
                    _ => _.ApplicationType,
                    _ => _.Receivers,
                    _ => _.Receivers.Select(r => r.Receiver));
            return applications;
        }

        public IEnumerable<Application> IncomingAppsApplications(bool forwarded = false, bool? received = null, bool archived = false)
        {
            var user = _userRepository.Find(c => c.UserId == UserId).SingleOrDefault();
            if (user == null)
            {
                return null;
            }
            var applications = _applicationRepo
                .FindWithNavProps(a =>
                        a.Receivers
                            .FirstOrDefault(r
                                => r.ReceiverId == user.DepartmentId && r.Forwarded == forwarded &&
                                   r.Received == received)
                            .ReceiverId == user.DepartmentId,
                    _ => _.ApplicationType,
                    _ => _.Receivers,
                    _ => _.Receivers.Select(r => r.Receiver)).Where(a => a.Archive == archived).ToList();
            return applications;
        }

        public IEnumerable<Application>ApplicationsToSign(bool? signed = null)
        {
            var user = _userRepository.Find(c => c.UserId == UserId).SingleOrDefault();
            if (user == null)
            {
                return null;
            }
            var applications = _applicationRepo
                .FindWithNavProps(
                    a => a.Approvals.FirstOrDefault(r => r.UserId == user.Id && r.Approve == signed).UserId ==
                         user.Id, _ => _.Approvals, _ => _.ApplicationType).ToList();
            foreach (var application in applications)
            {
                application.Approvals = application.Approvals.Where(c => c.UserId == user.Id).ToList();
            }

            return applications;
        }

        public IEnumerable<Application> ApplicationsToApprove(bool? signed = null, bool? approved = null, bool sendToHead = true)
        {
            var user = _userRepository.Find(c => c.UserId == UserId).SingleOrDefault();
            if (user == null)
            {
                return null;
            }
            var applications = _applicationRepo
                .FindWithNavProps(
                    a => a.Approvals.FirstOrDefault(r => r.UserId == user.Id && r.Approve == signed).UserId ==
                        user.Id && a.SendToHead == sendToHead && a.Approve == approved, _ => _.Approvals, _ => _.ApplicationType).ToList();
            foreach (var application in applications)
            {
                application.Approvals = application.Approvals.Where(c => c.UserId == user.Id).ToList();
            }

            return applications;
        }

        private static Activity ActivityLog(int userId, string actionPerformed)
        {
            return new Activity
            {
                UserId = userId,
                Action = actionPerformed,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        private static Approval NewApproval(int appId, int userId)
        {
            return new Approval
            {
                ApplicationId = appId,
                InviteDate = DateTime.Now,
                UserId = userId
            };
        }
    


    private static Approval UpdateApproval(Approval model, string remark, bool approve)
        {
            model.Remark = remark;
            model.Approve = approve;
            model.Date = DateTime.Now;

            return model;
        }

    private static ApplicationReceiver Recipient(int departmentId)
    {
        return new ApplicationReceiver
        {
            ReceiverId = departmentId,
            TimeSent = DateTime.Now
        };
    }
    }


}