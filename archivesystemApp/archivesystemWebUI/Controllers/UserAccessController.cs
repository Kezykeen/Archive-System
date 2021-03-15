using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace archivesystemWebUI.Controllers
{

    [Authorize(Roles = "Admin, Manager")]
    public class UserAccessController : Controller
    {
        #region FIELD
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserAccessService _service;
        #endregion

        #region CONSTRUCTOR
        public UserAccessController(IUnitOfWork unitOfWork, IUserAccessService service)
        {
            _unitOfWork = unitOfWork;
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
                return PartialView("_AddUserAccess", model);
            }

            try
            {
                await _service.AddUser(model);

                return Json("success", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                model.AccessLevels = _service.AccessLevels;
                return PartialView("_AddUserAccess", model);
            }
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
                return PartialView("_EditUserAccess", model);
            }

            try
            {
                await _service.Update(model);
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                model.AccessLevels = _service.AccessLevels;
                return PartialView("_EditUserAccess", model);
            }
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
            try
            {
                await _service.Delete(id);

                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }

        #endregion


        #region Validators

       
        [HttpPost]
        public JsonResult ValidateEmail(string Email)
        {
            _service.ValidateEmail(Email, out AccessDetail accessDetails, out Employee employee);

            if (employee == null)
                return Json("Employee with this email does not exists. Please enter a different email",  JsonRequestBehavior.AllowGet);

            return accessDetails == null? Json(true, JsonRequestBehavior.AllowGet) : Json("User already has an access level!", JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}