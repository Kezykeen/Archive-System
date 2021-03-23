using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models.DataLayers;
using AutoMapper;
using Microsoft.AspNet.Identity;

namespace archivesystemWebUI.Controllers.Api
{
    public class TicketsController: ApiController
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
            var user =  _unitOfWork.UserRepo.Find( c => c.UserId == currentUserId).SingleOrDefault();
            if (user == null)
            {
                return BadRequest();
            }

            var applications = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.UserId == user.Id, 
                    _ => _.ApplicationType,
                    _ => _.Receivers,
                    _=> _.Receivers.Select(r => r.Receiver));

          
            var response = Mapper.Map<IEnumerable<ApplicationsDataView>>(applications);

            return Ok(response);
        }

        [Route("api/incomingapplications")]
        public IHttpActionResult GetIncomingApplications(bool forwarded = false)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(c => c.UserId == currentUserId).SingleOrDefault();
            var applications = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => 
                    a.Receivers
                        .FirstOrDefault(r 
                            => r.ReceiverId == user.DepartmentId && r.Forwarded == forwarded)
                        .ReceiverId == user.DepartmentId,
                    _ => _.ApplicationType,
                    _ => _.Receivers,
                    _ => _.Receivers.Select(r => r.Receiver)).ToList();
            if (!forwarded)
            {
              applications =  _unitOfWork.ApplicationRepo
                    .FindWithNavProps(a => a.Receivers
                            .FirstOrDefault(r => r.ReceiverId == user.DepartmentId 
                                                 && r.Forwarded == forwarded )
                            .ReceiverId == user.DepartmentId,
                        _ => _.ApplicationType,
                        _ => _.Receivers,
                        _ => _.Receivers.Select(r => r.Receiver)).ToList();
            }

           
            var response = Mapper.Map<IEnumerable<ApplicationsDataView>>(applications);
            return Ok(response);
        }

        [Route("api/applicationstoapprovals")]
        public IHttpActionResult GetApplicationsForApproval(bool? approved = null)
        {
            var currentUserId = User.Identity.GetUserId();
            var user = _unitOfWork.UserRepo.Find(c => c.UserId == currentUserId).SingleOrDefault();
            var response = _unitOfWork.ApplicationRepo
                .FindWithNavProps(a => a.Approvals.FirstOrDefault(r => r.UserId == user.Id && r.Approve == approved).UserId == user.Id).ToList();
            switch (approved)
            {
                case true:
                    response = _unitOfWork.ApplicationRepo
                        .FindWithNavProps(a => a.Approvals.FirstOrDefault(r => r.UserId == user.Id && r.Approve == approved).UserId == user.Id).ToList();
                    break;
                case false:
                    response =  _unitOfWork.ApplicationRepo
                        .FindWithNavProps(a => a.Approvals.FirstOrDefault(r => r.UserId == user.Id && r.Approve == approved).UserId == user.Id).ToList();
                    break;
            }

           
            return Ok(response);
        }

        [Route("api/departments/")]
        public IHttpActionResult GetDepartments(string searchTerm = null)
        {
            var departments = _unitOfWork.DeptRepo.Find(u => u.Name.Contains(searchTerm)).Select(x => new
            {
                id = x.Id,
                text = x.Name
            });
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                departments = _unitOfWork.DeptRepo.GetAll().Select(x => new
                {
                    id = x.Id,
                    text = x.Name
                });
            }

            return Ok(departments);
        }
    }
}