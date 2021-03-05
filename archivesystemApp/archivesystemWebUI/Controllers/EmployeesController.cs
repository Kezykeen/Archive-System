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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace archivesystemWebUI.Controllers
{
    
    public class EmployeesController : Controller
    {


        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

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
        public EmployeesController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
      
        // GET: Employees
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult EditModal(int id)
        {
            var employee = _unitOfWork.EmployeeRepo.GetEmployeeWithDept(id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            var vm = Mapper.Map<UpdateEmployeeVM>(employee);
            vm.Departments = _unitOfWork.DeptRepo.GetAll();
            return PartialView(vm);
        }


        public ActionResult EnrollModal()
        {
            var model = new EnrollViewModel
            {
                Departments = _unitOfWork.DeptRepo.GetAll()
            };

            return PartialView(model);
        }

        public ActionResult Details(int id)
        {
            var employee = _unitOfWork.EmployeeRepo.GetEmployeeWithDept(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Enroll( EnrollViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = _unitOfWork.DeptRepo.GetAll();
                return PartialView("EnrollModal", model);
            }

            var uniqueProps = Mapper.Map<EnrollViewModel, EmpUniqueProps>(model);
            var (msg, propExists) = EmployeeValidator.Validate(_unitOfWork, uniqueProps);

            if (propExists)
            {
                ModelState.AddModelError("", msg);
                model.Departments = _unitOfWork.DeptRepo.GetAll();
                return PartialView("EnrollModal", model);
            }

            var employee = Mapper.Map<EnrollViewModel, Employee>(model);
            _unitOfWork.EmployeeRepo.Add(employee);

            string code = _tokenGenerator.Code(employee.Id);
            await _unitOfWork.SaveAsync();
            var callbackUrl = Url.Action("Register", "Account", new { userId = employee.Id, code = code }, protocol: Request.Url.Scheme);
            try
            {
                await _emailSender
                    .SendEmailAsync(employee.Email, 
                        "Complete Your Registration",
                        $"Hi, {employee.Name} complete your Enrollment Process by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            catch (Exception e)
            {
               ModelState.AddModelError("", e.Message);
               model.Departments = _unitOfWork.DeptRepo.GetAll();
               return PartialView("EnrollModal",model);
            }

            return Json(new { status = "success"});
        }

     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UpdateEmployeeVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = _unitOfWork.DeptRepo.GetAll();
                model.Roles = _roleService.GetAllRoles();
                return PartialView("EditModal", model);
            }
            var employeeDb = _unitOfWork.EmployeeRepo.GetEmployeeWithDept(model.Id);
            var Name = $"{model.FirstName} {model.LastName}";

            if (!string.IsNullOrWhiteSpace(employeeDb.UserId))
            {
                var rolesDb = UserManager.FindById(employeeDb.UserId).Roles;
                var roleDbId = rolesDb.SingleOrDefault()?.RoleId;
                var appUser = UserManager.FindById(employeeDb.UserId);

                if (!String.Equals(model.Email, appUser.Email, StringComparison.OrdinalIgnoreCase))
                {
                    appUser.Email = model.Email;
                }
                else if(!String.Equals(Name, appUser.UserName, StringComparison.OrdinalIgnoreCase))
                {

                    appUser.UserName = Name;
                }else if(!String.Equals(model.Phone, appUser.PhoneNumber, StringComparison.OrdinalIgnoreCase))
                {
                    appUser.PhoneNumber = model.Phone;
                }

                UserManager.Update(appUser);

                if (!string.IsNullOrWhiteSpace(roleDbId) && model.RoleId != roleDbId)
                {
                    UserManager.RemoveFromRole(employeeDb.UserId, RoleManager.FindById(roleDbId).Name);
                    UserManager.AddToRole(employeeDb.UserId, RoleManager.FindById(model.RoleId).Name);
                }

            }

            Mapper.Map(model, employeeDb);
            employeeDb.UpdatedAt = DateTime.Now;
            _unitOfWork.Save();

            return Json(new { status = "success", msg = $"{model.FirstName}" });

        }



        public JsonResult IsEmailTaken(string email, int? id = null)
        {
            // Check if the e-mail already exists
            return Json(!_unitOfWork.EmployeeRepo.EmailExists(email, id), JsonRequestBehavior.AllowGet);
        }


        public JsonResult IsPhoneTaken(string phone, int? id = null)
        {
            // Check if the phone already exists
            return Json(!_unitOfWork.EmployeeRepo.PhoneExists(phone, id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsNameTaken(string firstName, string lastName, int? id = null)
        {
            // Check if the Name already exists
            return Json(!_unitOfWork.EmployeeRepo.NameExists($"{firstName} {lastName}", id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsStaffIdTaken(string staffId, int? id =null)
        {
            // Check if the staffId already exists
            return Json(!_unitOfWork.EmployeeRepo.StaffIdExists(staffId, id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DelPrompt(int id)
        {
            return PartialView(_unitOfWork.EmployeeRepo.Get(id));
        }


        public async Task<ActionResult> ResendConfirmation(int id, string email, string name)
        {
            string code = _tokenGenerator.Code(id);
            _unitOfWork.Save();
            var callbackUrl = Url.Action("Register", "Account", new { userId = id, code = code }, protocol: Request.Url.Scheme);
            try
            {
                await _emailSender.SendEmailAsync(email, "Complete Your Registration", $"Hi, {name} complete your Enrollment Process by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            catch (Exception e)
            {
                TempData["ConfFailed"] = $"{e.Message}";
                return RedirectToAction("Details", new {id=id});
            }
            TempData["ConfSent"] = "Confirmation Link Sent";
            return RedirectToAction("Details", new { id = id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }
            }


            base.Dispose(disposing);
        }

    }
}