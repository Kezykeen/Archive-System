using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleService _roleService;

        public EmployeesController(IUnitOfWork unitOfWork, IRoleService roleService)
        {
            _unitOfWork = unitOfWork;
            _roleService = roleService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Enroll( EnrollViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
          
            var uniqueProps = Mapper.Map<EnrollViewModel, EmpUniqueProps>(model);
            var (msg, propExists) = EmployeeValidator.Validate(_unitOfWork, uniqueProps);

            if (propExists)
            {
                ModelState.AddModelError("", msg);
                model.Departments = _unitOfWork.DeptRepo.GetAll();
                return View(model);
            }
            
            var employee = Mapper.Map<EnrollViewModel, Employee>(model);
            _unitOfWork.EmployeeRepo.Add(employee);
           await _unitOfWork.SaveAsync();
           return RedirectToAction("Index", "Home");

          
        }
    }
}