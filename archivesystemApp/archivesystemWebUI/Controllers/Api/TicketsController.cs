using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models.DataLayers;
using AutoMapper;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace archivesystemWebUI.Controllers.Api
{
    public class TicketsController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("api/tickets")]
        [HttpGet]
        public IHttpActionResult GetAllTickets()
        {
            var tickets = _unitOfWork.TicketRepo.GetAll();
            var response = Mapper.Map<IEnumerable<Ticket>, List<TicketDataView>>(tickets);

            return Ok(response);

        }


        [Route("api/myapplications")]
        public IHttpActionResult GetMyApplications()
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(c => c.UserId == currentUserId).SingleOrDefault();
            if (user == null)
            {
                return BadRequest();
            }

            var applications = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.UserId == user.Id,
                    _ => _.ApplicationType,
                    _ => _.Receivers,
                    _ => _.Receivers.Select(r => r.Receiver));


            var response = Mapper.Map<IEnumerable<ApplicationsDataView>>(applications);

            return Ok(response);
        }

        [Route("api/incomingapplications")]
        public IHttpActionResult GetIncomingApplications(bool forwarded = false, bool? received = null, bool archived = false)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(c => c.UserId == currentUserId).SingleOrDefault();
            var applications = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a =>
                        a.Receivers
                            .FirstOrDefault(r
                                => r.ReceiverId == user.DepartmentId && r.Forwarded == forwarded &&
                                   r.Received == received)
                            .ReceiverId == user.DepartmentId,
                    _ => _.ApplicationType,
                    _ => _.Receivers,
                    _ => _.Receivers.Select(r => r.Receiver)).Where(a => a.Archive == archived).ToList();


            var response = Mapper.Map<IEnumerable<ApplicationsDataView>>(applications);
            return Ok(response);
        }

        [Route("api/applicationstosign")]
        public IHttpActionResult GetApplicationsToSign(bool? signed = null)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(c => c.UserId == currentUserId).SingleOrDefault();
            var applications = _unitOfWork.ApplicationRepo
                .FindWithNavProps(
                    a => a.Approvals.FirstOrDefault(r => r.UserId == user.Id && r.Approve == signed).UserId == 
                         user.Id,_ => _.Approvals, _=> _.ApplicationType).ToList();
            foreach (var application in applications)
            {
                application.Approvals = application.Approvals.Where(c => c.UserId == user.Id).ToList();
            }
            var response =   Mapper.Map<IEnumerable<ApplicationsToSignDataView>>(applications);
            
            

            return Ok(response);
        }

        [Route("api/applicationstoapprove")]
        public IHttpActionResult GetApplicationsForApproval(bool? signed = null, bool? approved = null, bool sendToHead =true)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(c => c.UserId == currentUserId).SingleOrDefault();
            var applications = _unitOfWork.ApplicationRepo
                .FindWithNavProps(
                    a => a.Approvals.FirstOrDefault(r => r.UserId == user.Id && r.Approve == signed).UserId ==
                         user.Id && a.SendToHead ==sendToHead && a.Approve ==approved, _ => _.Approvals, _ => _.ApplicationType).ToList();
            foreach (var application in applications)
            {
                application.Approvals = application.Approvals.Where(c => c.UserId == user.Id).ToList();
            }
            var response = Mapper.Map<IEnumerable<ApplicationsToSignDataView>>(applications);



            return Ok(response);
        }

        [Route("api/departments/{id}/")]
        public IHttpActionResult GetDepartments(int id, string searchTerm = null)
        {
            var departments = _unitOfWork.DeptRepo.Find(d => d.Name.Contains(searchTerm) && d.Id != id).Select(x => new
            {
                id = x.Id,
                text = x.Name
            });
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                departments = _unitOfWork.DeptRepo.Find(d => d.Id != id).Select(x => new
                {
                    id = x.Id,
                    text = x.Name
                });
            }

            return Ok(departments);
        }
    }
}

