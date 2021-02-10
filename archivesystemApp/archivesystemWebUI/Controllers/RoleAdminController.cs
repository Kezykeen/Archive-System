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
    public class RoleAdminController : Controller
    {
        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        // GET: RoleAdmin
        public ActionResult Index()
        {
            //return View(RoleManager.Roles);
            return Content("This is the list of roles page");
        }

        //GET: /roleadmin/newrole
        [Route("roleadmin/newrole")]
        [HttpGet()]
        public ActionResult Create()
        {
            return View("NewUserRoleView");
        }


        //POST: /roleadmin/newrole
        [Route("roleadmin/newrole")]
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


        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        [HttpPost]
        public ActionResult Delete(string roleId)
        {
            var role=RoleManager.FindById(roleId);
            if (role != null)
                RoleManager.DeleteAsync(role);

            return RedirectToAction("Index");
        }
    }
}