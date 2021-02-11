using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;
using System.Threading.Tasks;
using archivesystemDomain.Interfaces;

namespace archivesystemWebUI.Controllers
{
    public class RolesController : Controller
    {
        private IUnitOfWork unitOfWork;
        public RolesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;  
        }
        

        // GET: /role
       
        public ActionResult Index()
        {
            return View(unitOfWork.RoleRepo.GetAllRoles());
        }

        //GET: /role/add 
        [Route("roles/add")]
        [HttpGet()]
        public ActionResult Create()
        {
            return View("NewUserRoleView");
        }

        //POST: /role/add
        [Route("roles/add")]
        [HttpPost]
        public async Task<ActionResult> Create(NewRoleViewModel _role)
        {
            var result=await unitOfWork.RoleRepo.AddRole(_role.Name);
            if (!result.Succeeded)
            {
                AddErrorsFromResult(result);
                return View("NewUserRoleView", _role);
            }
                
            return RedirectToAction("Index");
        }

        //POST: /role/delete
        [HttpPost]
        public ActionResult Delete(string roleId)
        {
            unitOfWork.RoleRepo.DeleteRole(roleId); 
            return RedirectToAction("Index");
        }

        //POST: /roleadmin/getusers
        //[HttpPost]
        //public ActionResult GetUsers(string roleId)
        //{
        //    ApplicationRole role=RoleManager.FindById(roleId);
        //    var users=role.Users;
        //    List<>
        //    foreach(ApplicationUser user in users)
        //    {
        //      HttpContext.User.Identity.Name
        //    }
        //}

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        
    }
}