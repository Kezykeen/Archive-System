using System;
using System.Linq;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ITicketService _ticketService;
    

        public SettingsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
           
        }
        public ActionResult TicketTypes()
        {
            return View();
        }

        public ActionResult TicketTypeModal()
        {
            return PartialView(new TicketViewModel{Status = Status.Active});
        }

        public ActionResult EditTicket(int id)
        {
            var model = _ticketService.GetTicket(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var ticketVm=  Mapper.Map<TicketViewModel>(model);
            return PartialView("TicketTypeModal", ticketVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveTicket(TicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("TicketTypeModal", model);
            }

            if (model.Id == 0)
            {
                var nameExists = _ticketService.NameExists(model, null);
             if (nameExists)
             {
                 ModelState.AddModelError("name", "Name Exists");
                 return PartialView("TicketTypeModal", model);

             }

             var ticket = Mapper.Map<Ticket>(model);

             _ticketService.Create(ticket);
             return Json(new { saved = true });
            }

            var nameExist = _ticketService.NameExists(model, model.Id);
            if (nameExist)
            {
                ModelState.AddModelError("name", "Name Exists");
                return PartialView("TicketTypeModal", model);
            }
            var ticketDb = _ticketService.GetTicket(model.Id);

            if (ticketDb == null)
            {
                return HttpNotFound();
            }

            _ticketService.Update(model, ticketDb);

            return Json(new { updated = true });

        }

    }
}