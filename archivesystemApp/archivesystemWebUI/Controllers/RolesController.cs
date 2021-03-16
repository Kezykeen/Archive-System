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
        private readonly IUnitOfWork repo;
     
        public RolesController(IRoleService service, IUnitOfWork repo)
        {
            _service = service;
            this.repo = repo;
        }
        

        // GET: /roles
        public ActionResult Index()
        {
            return View("RolesList",_service.GetAllRoles());
        }

        //GET: /roles/manage
        [Route("roles/manage")]
        [HttpGet()]
        public ActionResult Manage(string name, string id)
        {
            Guid _id = Guid.Parse( string.IsNullOrEmpty(id) ? new Guid().ToString() : id);
            if(_id== new Guid())
                return PartialView("_AddEditRole", new AddOrEditRoleViewModel());

            return PartialView("_AddEditRole",new AddOrEditRoleViewModel() { Name = name, Id = _id });
        }

        //POST: /roles/manage
        [Route("roles/manage")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public async Task<ActionResult> AddOrEdit(AddOrEditRoleViewModel _role)
        {
            IdentityResult result;
            if (string.IsNullOrEmpty(_role.Name))
                result = await _service.AddRole(_role.NewName);
            else
                result = await _service.EditRole(_role.Name,_role.NewName);
            if (!result.Succeeded)
            {
                if (result.Errors.ToList()[0].Contains("is already taken"))
                    return new HttpStatusCodeResult(400);
                else
                    return new HttpStatusCodeResult(500);
            }
                
            return new HttpStatusCodeResult(200);
        }

        [Route("roles/delete")]
        [HttpGet]
        public ActionResult GetDeletePartial(string name, string id)
        {
            Guid idInGuid = Guid.Parse(string.IsNullOrEmpty(id) ? new Guid().ToString() : id);
            if (idInGuid != new Guid())
                return PartialView("_DeleteRole", new AddOrEditRoleViewModel() { Name = name, Id = idInGuid });

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

        //GET: /roles/users
        [Route("roles/users")]
        [HttpGet]
        public async Task<ActionResult> GetUsers(string roleName)
        {
            var userIds=await _service.GetUserIdsOfUsersInRole(roleName);
            var usersData=repo.UserRepo.GetUserDataByUserIds(userIds);
            var viewModel=GetUsersInRoleViewModel(usersData, roleName);
            return View("UsersInRole",viewModel);
        }

        private UsersInRoleViewModel GetUsersInRoleViewModel(IEnumerable<RoleMemberData> usersData,string roleName)
        {
            var viewModel = new UsersInRoleViewModel
            {
                RoleName = roleName,
                Users = usersData
            };
            return viewModel;
        }








    }
}