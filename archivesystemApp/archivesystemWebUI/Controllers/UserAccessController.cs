using archivesystemDomain.Entities;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace archivesystemWebUI.Controllers
{

    [Authorize(Roles = "Admin, Manager")]
    public class UserAccessController : Controller
    {
        #region FIELD
        private readonly IUserAccessService _service;
        #endregion

        #region CONSTRUCTOR
        public UserAccessController( IUserAccessService service)
        {
            _service = service;
        }

        #endregion

        #region ACTION METHODS
        /// <summary>
        /// This region contains all the actions to manage creation and manipulation of User Access Details
        /// 
        /// These actions make use of "AccessDetails" table
        /// </summary>
        [Route("UserAccess")]
        public ActionResult ManageUserAccess()
        {
            var model = _service.AccessDetails;
            return View(model);
        }

        public ActionResult AddUser()
        {
            var viewModel = _service.AddUserModel();
            return PartialView("_AddUserAccess", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(AddUserToAccessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccessLevels = _service.AccessLevels;
                return Json(new { model }, JsonRequestBehavior.AllowGet);
            }
            
            var data = await _service.AddUser(model);
            if (data.Item2 == null)
            {
                return Json(data.Item1, JsonRequestBehavior.AllowGet);
            }
            
            ModelState.AddModelError("", data.Item2);
            model.AccessLevels = _service.AccessLevels;
            return Json(new { model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditUser(int id)
        {
            _service.EditUserModel(id, out EditUserViewModel model, out AccessDetail accessDetails);
            if (accessDetails == null)
                return HttpNotFound();
                       
            return PartialView("_EditUserAccess", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccessLevels = _service.AccessLevels;
                return Json(new { model }, JsonRequestBehavior.AllowGet);
            }

            var data = await _service.UpdateUser(model);
            if (data.Item2 == null)
            {
                return Json(data.Item1, JsonRequestBehavior.AllowGet);
            }

            ModelState.AddModelError("", data.Item2);
            model.AccessLevels = _service.AccessLevels;
            return Json(new { model }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ManageUserAccess));
            }

            var model =_service.GetByNullableId(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            return PartialView("_DeleteUserAccess", model);
        }

        [HttpPost, ActionName(nameof(DeleteUser))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var data = await _service.Delete(id);
            return Json(data, JsonRequestBehavior.AllowGet);            
        }
        #endregion


        #region Validators
        [HttpPost]
        public JsonResult ValidateEmail(string Email)
        {
            var user = _service.GetUserByEmail(Email);
            if (user == null)
                return Json("User with this email does not exists. Please enter a different email", JsonRequestBehavior.AllowGet);

            var accessDetails = _service.GetByAppUserId(user.Id);
            return accessDetails == null ? Json(true, JsonRequestBehavior.AllowGet) : Json("User already has an access level!", JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}