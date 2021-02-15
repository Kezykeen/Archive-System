using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleService _roleService;
        private readonly IEmailSender _emailSender;
        private readonly ITokenGenerator _tokenGenerator;

        public EmployeesController(
            IUnitOfWork unitOfWork,
            IRoleService roleService,
            IEmailSender emailSender, 
            ITokenGenerator tokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _roleService = roleService;
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;
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

            string code = _tokenGenerator.Code(employee.Id);
            await _unitOfWork.SaveAsync();
            var callbackUrl = Url.Action("Register", "Account", new { userId = employee.Id, code = code }, protocol: Request.Url.Scheme);
            try
            {
                await _emailSender.SendEmailAsync(employee.Email, "Complete Your Registration", $"Hi, {employee.Name} complete your Enrollment Process by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            catch (Exception e)
            {
               ModelState.AddModelError("", e.Message);
               model.Departments = _unitOfWork.DeptRepo.GetAll();
               return View(model);
            }

            return RedirectToAction("Index", "Home");

          
        }
    }
}