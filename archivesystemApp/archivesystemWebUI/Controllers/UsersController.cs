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
using static System.String;

namespace archivesystemWebUI.Controllers
{

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IRoleService _roleService;


        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly ITokenGenerator _tokenGenerator;
       


        public UsersController(
            IUnitOfWork unitOfWork,
            IEmailSender emailSender,
            ITokenGenerator tokenGenerator,
            IRoleService roleService)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _tokenGenerator = tokenGenerator;
            _roleService = roleService;
        }
        public UsersController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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
        [Authorize(Roles = "Admin")]
        // GET: Employees
        public ActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "Admin")]
        public ActionResult EditModal(int id)
        {
            var user = _unitOfWork.UserRepo.GetUserWithDept(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var vm = Mapper.Map<UpdateUserVm>(user);
            vm.Departments = _unitOfWork.DeptRepo.GetAll();
            return PartialView(vm);
        }

        [Authorize(Roles = "Admin")]
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
            var user = _unitOfWork.UserRepo.GetUserWithDept(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        [Authorize(Roles = "Admin")]
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
            var (msg, propExists) = UserValidator.Validate(_unitOfWork, uniqueProps);

            if (propExists)
            {
                ModelState.AddModelError("", msg);
                model.Departments = _unitOfWork.DeptRepo.GetAll();
                return PartialView("EnrollModal", model);
            }

            var user = Mapper.Map<EnrollViewModel, AppUser>(model);
            _unitOfWork.UserRepo.Add(user);

            string code = _tokenGenerator.Code(user.Id);
            await _unitOfWork.SaveAsync();
            var callbackUrl = Url.Action("Register", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            try
            {
                await _emailSender
                    .SendEmailAsync(user.Email, 
                        "Complete Your Registration",
                        $"Hi, {user.Name} complete your Enrollment Process by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            catch (Exception e)
            {
               ModelState.AddModelError("", e.Message);
               model.Departments = _unitOfWork.DeptRepo.GetAll();
               return PartialView("EnrollModal",model);
            }

            return Json(new { status = "success"});
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UpdateUserVm model)
        {
            if (!ModelState.IsValid)
            {
                model.Departments = _unitOfWork.DeptRepo.GetAll();
                return PartialView("EditModal", model);
            }
            var userDb = _unitOfWork.UserRepo.GetUserWithDept(model.Id);
            var name = $"{model.FirstName} {model.LastName}";

            if (!IsNullOrWhiteSpace(userDb.UserId))
            {
                
                var appUser = UserManager.FindById(userDb.UserId);

                if (!string.Equals(model.Email, appUser.Email, StringComparison.OrdinalIgnoreCase))
                {
                    appUser.Email = model.Email;
                }
                else if(!string.Equals(name, appUser.UserName, StringComparison.OrdinalIgnoreCase))
                {

                    appUser.UserName = name;
                }else if(!string.Equals(model.Phone, appUser.PhoneNumber, StringComparison.OrdinalIgnoreCase))
                {
                    appUser.PhoneNumber = model.Phone;
                }

                UserManager.Update(appUser);

               

            }

            Mapper.Map(model, userDb);
            userDb.UpdatedAt = DateTime.Now;
            _unitOfWork.Save();

            return Json(new { status = "success", msg = $"{model.FirstName}" });

        }



        public JsonResult IsEmailTaken(string email, int? id = null)
        {
            // Check if the e-mail already exists
            return Json(!_unitOfWork.UserRepo.EmailExists(email, id), JsonRequestBehavior.AllowGet);
        }


        public JsonResult IsPhoneTaken(string phone, int? id = null)
        {
            // Check if the phone already exists
            return Json(!_unitOfWork.UserRepo.PhoneExists(phone, id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsNameTaken(string firstName, string lastName, int? id = null)
        {
            // Check if the Name already exists
            return Json(!_unitOfWork.UserRepo.NameExists($"{firstName} {lastName}", id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsIdTaken(string tagId, int? id =null)
        {
            // Check if the staffId already exists
            return Json(!_unitOfWork.UserRepo.TagIdExists(tagId, id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DelPrompt(int id)
        {
            return PartialView(_unitOfWork.UserRepo.Get(id));
        }

        [Authorize(Roles = "Admin")]
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

       
        public  async Task<ActionResult> Officers(int id ,string searchTerm)
        {
            var currentUserId = User.Identity.GetUserId();
            var usersInDept = await _roleService.GetUserIdsOfUsersInRole("DeptOfficer");
           var users = _unitOfWork.UserRepo
                .Find( u => u.DepartmentId == id && usersInDept.Contains(u.UserId) && u.UserId != currentUserId && u.Name.Contains(searchTerm) && usersInDept.Contains(u.UserId)).Select(x => new
            {
                id = x.Id,
                text = x.Name
            });
            if (IsNullOrWhiteSpace(searchTerm))
            {
               
                users = _unitOfWork.UserRepo.Find(u => u.DepartmentId == id && usersInDept.Contains(u.UserId) && u.UserId != currentUserId
                ).Select(x => new
                {
                    id = x.Id,
                    text = x.Name
                });
            }
         
            return Json(users, JsonRequestBehavior.AllowGet);
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