using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: Employees
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Enroll()
        {
            var model = new EnrollViewModel
            {
                Departments = _unitOfWork.DeptRepo.GetAll()
            };

            return View(model);
        }

    }
}