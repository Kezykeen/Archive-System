using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Models;
using System.Threading.Tasks;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models.RoleViewModels;
using archivesystemDomain.Services;

namespace archivesystemWebUI.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class RolesController : Controller
    {
        private readonly IRoleService _service;
     
        public RolesController(IRoleService service)
        {
            _service = service;
        }

        [Route("roles/manage")]
        [HttpGet()]
        public ActionResult AddOrEdit(string name, string id)
        {
            if (string.IsNullOrEmpty(id))
                return PartialView("_AddEditRole", new AddOrEditRoleViewModel());

            return PartialView("_AddEditRole", new AddOrEditRoleViewModel() { Name = name, Id = id });
        }

        //POST: /roles/manage
        [Route("roles/manage")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<ActionResult> AddOrEdit(AddOrEditRoleViewModel _role)
        {
            IdentityResult result =
                string.IsNullOrEmpty(_role.Name) ? await _service.AddRole(_role.NewName)
                                                 : await _service.EditRole(_role.Name, _role.NewName);
            if (!result.Succeeded)
            {
                if (result.Errors.ToList()[0].Contains("is already taken"))
                    return new HttpStatusCodeResult(400);
                else
                    return new HttpStatusCodeResult(500);
            }

            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        public ActionResult AddUserToRole(string userId)
        {
           var roles=_service.GetAllRoles();
            if(string.IsNullOrWhiteSpace(userId))
                return PartialView("_AddUserToRole", new AddToRoleViewModel{Roles = roles});
            var employeeName = _service.GetEmployeeName(userId);
            return PartialView("_AddUserToRole",new AddToRoleViewModel {
                Roles = roles, UserId = userId, EmployeeName=employeeName });
        }

        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult AddUserToRole(string userId, string roleId,string userEmail)
        {
            IdentityResult result;
            if (string.IsNullOrWhiteSpace(roleId)) return new HttpStatusCodeResult(403);
            if (string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(userEmail))
                result=AddUserToRoleByEmail(userEmail, roleId);
            else
                result= _service.AddToRole(userId, roleId);
            if(result.Succeeded) return new HttpStatusCodeResult(201);

            if (result.Errors.Any(x => x.Contains("User already in role"))) return new HttpStatusCodeResult(400);
            if (result.Errors.Any(x => x.Contains("user does not exist"))) return new HttpStatusCodeResult(403);
            

            return new HttpStatusCodeResult(500);
        }

        //POST: /role/delete
        [Route("roles/delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string roleId)
        {
            _service.DeleteRole(roleId);
            return RedirectToAction("Index");
        }

        [Route("roles/delete")]
        [HttpGet]
        public ActionResult GetDeletePartial(string name, string id)
        {

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(500);

            return PartialView("_DeleteRole", new AddOrEditRoleViewModel() { Name = name, Id = id });
        }

        //GET: /roles/users
        [Route("roles/users")]
        [HttpGet]
        public async Task<ActionResult> GetUsers(string roleName)
        {
            var userIds = await _service.GetUserIdsOfUsersInRole(roleName);
            var usersData = _service.GetUsersData(userIds);
            var viewModel = GetUsersInRoleViewModel(usersData, roleName);
            return View("UsersInRole", viewModel);
        }

        // GET: /roles
        public ActionResult Index()
        {
            return View("RolesList",_service.GetAllRoles());
        }

        [HttpGet]
        public ActionResult RemoveUserFromRole(RemoveUserFromRoleViewModel model)
        {
            model.EmployeeName=_service.GetEmployeeName(model.UserId);
            if(ModelState.IsValid)
                return PartialView("_RemoveUserFromRole", model);

            return new HttpStatusCodeResult(500);
        }

        [HttpPost]
        public ActionResult RemoveUserFromRole(string userId, string roleName)
        {
             var result= _service.RemoveFromRole(userId, roleName);
            if (result.Succeeded)
                return RedirectToAction(nameof(GetUsers), new { roleName });

            return new HttpStatusCodeResult(500);
        }



        #region Private Methods

        private UsersInRoleViewModel GetUsersInRoleViewModel(IEnumerable<RoleMemberData> usersData,string roleName)
        {
            var viewModel = new UsersInRoleViewModel
            {
                RoleName = roleName,
                Users = usersData
            };
            return viewModel;
        }

        private IdentityResult AddUserToRoleByEmail(string userEmail, string roleId)
        {
           var result =_service.AddToRoleByEmail(userEmail, roleId);
            return result;
        } 

        #endregion





    }
}