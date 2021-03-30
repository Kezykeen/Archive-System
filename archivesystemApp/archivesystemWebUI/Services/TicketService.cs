using System;
using System.Collections.Generic;
using System.Linq;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketRepo _ticketRepo;

        public TicketService(IUnitOfWork unitOfWork, ITicketRepo ticketRepo)
        {
            _unitOfWork = unitOfWork;
            _ticketRepo = ticketRepo;
        }

        public IEnumerable<Ticket> GetAll()
        {
            return _ticketRepo.GetAll();
        }

        public Ticket GetTicket(int id)
        {
            return _ticketRepo.Get(id);
        }

        public bool NameExists(TicketViewModel model, int? ticketId)
        {


            if (ticketId == null) return _ticketRepo.GetAll().Any(e => string.Equals(e.Name, model.Name,
                StringComparison.OrdinalIgnoreCase));

            return _ticketRepo.GetAll().Any(e => string.Equals(e.Name, model.Name,
                StringComparison.OrdinalIgnoreCase) &&  e.Id != ticketId.Value);
        }

        public bool Create(Ticket ticket)
        {
            _ticketRepo.Add(ticket);
            _unitOfWork.Save();
            return true;
        }

        public bool Update(TicketViewModel model, Ticket ticket)
        {
            Mapper.Map(model, ticket);
            _unitOfWork.Save();
            return true;
        }
    }
}