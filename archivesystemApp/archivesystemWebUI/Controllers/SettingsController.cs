using System;
using System.Linq;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var model = _unitOfWork.TicketRepo.Get(id);
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
             var nameExists =  _unitOfWork.TicketRepo.GetAll().Any(e => string.Equals(e.Name, model.Name,
                    StringComparison.OrdinalIgnoreCase));
             if (nameExists)
             {
                 ModelState.AddModelError("name", "Name Exists");
                 return PartialView("TicketTypeModal", model);

             }

             var ticket = Mapper.Map<Ticket>(model);
             _unitOfWork.TicketRepo.Add(ticket);
             _unitOfWork.Save();
             return Json(new { saved = true });
            }

            var nameExist = _unitOfWork.TicketRepo.GetAll()
                .Any(e => string.Equals(e.Name, model.Name,
                    StringComparison.OrdinalIgnoreCase) && e.Id != model.Id);
            if (nameExist)
            {
                ModelState.AddModelError("name", "Name Exists");
                return PartialView("TicketTypeModal", model);
            }
            var ticketDb = _unitOfWork.TicketRepo.Get(model.Id);

            if (ticketDb == null)
            {
                return HttpNotFound();
            }

            ticketDb = Mapper.Map(model, ticketDb);
            // ticketDb.UpdatedAt = DateTime.Now;
            _unitOfWork.Save();

            return Json(new { updated = true });

        }

    }
}