using System.Collections.Generic;
using System.Web.Http;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models.DataLayers;
using AutoMapper;

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
    }
}