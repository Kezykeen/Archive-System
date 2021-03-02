using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;
using System.Threading.Tasks;
using archivesystemDomain.Interfaces;

namespace archivesystemWebUI.Controllers
{
    public class RolesController : Controller
    {
        private readonly IRoleService _service;
     
        public RolesController(IRoleService service)
        {
            _service = service;
        }
        

        // GET: /role
       
        public ActionResult Index()
        {
            return View(_service.GetAllRoles());
        }

        //GET: /role/add 
        [Route("roles/add")]
        [HttpGet()]
        public ActionResult Create()
        {
            return PartialView("_AddEditRole",new RoleViewModel());
        }

        //POST: /role/add
        [Route("roles/add")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<ActionResult> Create(RoleViewModel _role)
        {
            var result=await _service.AddRole(_role.Name);
            if (!result.Succeeded)
            {
                if (result.Errors.ToList()[0].Contains("is already taken"))
                    return new HttpStatusCodeResult(400);
                else
                    return new HttpStatusCodeResult(500);
            }
                
            return new HttpStatusCodeResult(200);
        }

        //POST: /role/delete
        [HttpPost]
        public ActionResult Delete(string roleId)
        {
            _service.DeleteRole(roleId); 
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

        [Route("roles/edit")]
        public ActionResult Update()
        {
            string name=Request.QueryString["name"];

            Guid id=Guid.Parse(Request.QueryString["id"]);

            return PartialView("_AddEditRole", new RoleViewModel() { Name = name, Id=id }) ;
        }

        [HttpPost]
        public async Task<ActionResult> Update(EditRoleViewModel model)
        {
            var result= await _service.EditRole(model.OldName, model.NewName);
            if (result.Succeeded)
                return RedirectToAction("Index");

            AddErrorsFromResult(result);
            return View("EditRoleView", model);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        
    }
}