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

namespace archivesystemWebUI.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET: RoleAdmin
        public ActionResult Index()
        {
            var roles = RoleManager.Roles.ToList();
            return View(roles);
        }

        //GET: /roleadmin/newrole 
        [Route("role/add")]
        [HttpGet()]
        public ActionResult Create()
        {
            return View("NewUserRoleView");
        }

        //POST: /roleadmin/newrole
        [Route("role/add")]
        [HttpPost]
        public async Task<ActionResult> Create(NewRoleViewModel _role)
        {
            ApplicationRole role = new ApplicationRole(_role.Name);
            IdentityResult result=await RoleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                AddErrorsFromResult(result);
            }
            return RedirectToAction("Index");
        }

        //POST: /roleadmin/delete
        [HttpPost]
        public ActionResult Delete(string roleId)
        {
            var role = RoleManager.FindById(roleId);
            if (role != null)
                RoleManager.Delete(role);

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