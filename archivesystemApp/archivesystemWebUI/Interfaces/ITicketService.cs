using System.Collections.Generic;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Interfaces
{
    public interface ITicketService
    {
        IEnumerable<Ticket> GetAll();
        Ticket GetTicket(int id);
        bool NameExists(TicketViewModel model, int? ticketId);
        bool Create(Ticket ticket);
        bool Update(TicketViewModel model, Ticket ticket);
    }
}