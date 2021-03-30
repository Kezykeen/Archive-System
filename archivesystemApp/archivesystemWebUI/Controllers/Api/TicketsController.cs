using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models.DataLayers;
using AutoMapper;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace archivesystemWebUI.Controllers.Api
{
    public class TicketsController : ApiController
    {
        private readonly ITicketService _ticketService;
        private readonly IDepartmentService _departmentService;
        private readonly IApplicationService _applicationService;

        public TicketsController(
            ITicketService ticketService,
            IDepartmentService departmentService,
            IApplicationService applicationService
            )
        {
            _ticketService = ticketService;
            _departmentService = departmentService;
            _applicationService = applicationService;
        }

        [Route("api/tickets")]
        [HttpGet]
        public IHttpActionResult GetAllTickets()
        {
            var tickets = _ticketService.GetAll();
            var response = Mapper.Map<IEnumerable<Ticket>, List<TicketDataView>>(tickets);

            return Ok(response);

        }


        [Route("api/myapplications")]
        public IHttpActionResult GetMyApplications()
        {
            var applications = _applicationService.UserApplications();

            var response = Mapper.Map<IEnumerable<ApplicationsDataView>>(applications);

            return Ok(response);
        }

        [Route("api/incomingapplications")]
        public IHttpActionResult GetIncomingApplications(bool forwarded = false, bool? received = null, bool archived = false)
        {

            var applications = _applicationService.IncomingAppsApplications(forwarded, received, archived);

            var response = Mapper.Map<IEnumerable<ApplicationsDataView>>(applications);

            return Ok(response);
        }


        [Route("api/applicationstosign")]
        public IHttpActionResult GetApplicationsToSign(bool? signed = null)
        {

            var applications = _applicationService.ApplicationsToSign(signed);

            var response =   Mapper.Map<IEnumerable<ApplicationsToSignDataView>>(applications);

            return Ok(response);
        }

        [Route("api/applicationstoapprove")]
        public IHttpActionResult GetApplicationsForApproval(bool? signed = null, bool? approved = null, bool sendToHead =true)
        {

            var applications = _applicationService.ApplicationsToApprove(signed, approved, sendToHead);
            var response = Mapper.Map<IEnumerable<ApplicationsToSignDataView>>(applications);

            return Ok(response);
        }

        [Route("api/departments/{id}/")]
        public IHttpActionResult GetDepartments(int id, string searchTerm = null)
        {
            var departments = _departmentService.GetDepartments(id, searchTerm);

            return Ok(departments);
        }
    }
}

