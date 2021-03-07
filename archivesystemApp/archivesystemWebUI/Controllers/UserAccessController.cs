using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace archivesystemWebUI.Controllers
{
    public class UserAccessController : Controller
    {
        #region FIELD
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region CONSTRUCTOR
        public UserAccessController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region ACTION METHODS
        /// <summary>
        /// This region contains all the actions to manage creation and manipulation of User Access Details
        /// 
        /// These actions make use of "AccessDetails" table
        /// </summary>
        public ActionResult ManageUserAccess()
        {
            var model = _unitOfWork.AccessDetailsRepo.GetAccessDetails();
            return View(model);
        }

        public ActionResult AddUser()
        {
            var accessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            var viewModel = new AddUserToAccessViewModel { AccessLevels = accessLevels };
            return PartialView("_AddUserAccess", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(AddUserToAccessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return PartialView("_AddUserAccess", model);
            }

            try
            {
                var employee = _unitOfWork.EmployeeRepo.GetEmployeeByMail(model.Email);
               
                var newAccessDetails = new AccessDetail
                {
                    EmployeeId = employee.Id,
                    AccessLevelId = model.AccessLevel,
                    AccessCode = AccessCodeGenerator.NewCode(employee.StaffId),
                    Status = Status.Active
                };

                _unitOfWork.AccessDetailsRepo.Add(newAccessDetails);
                await _unitOfWork.SaveAsync();

                return Json("success", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return PartialView("_AddUserAccess", model);
            }
        }

        public ActionResult EditUser(int id)
        {
            var accessDetails = _unitOfWork.AccessDetailsRepo.Get(id);
            if (accessDetails == null)
                return HttpNotFound();
            var accessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            var model = new EditUserViewModel
            {
                RegenerateCode = CodeStatus.No,
                AccessDetails = accessDetails,
                AccessLevels = accessLevels
            };
            return PartialView("_EditUserAccess", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return PartialView("_EditUserAccess", model);
            }
            try
            {
                if (model.RegenerateCode == CodeStatus.Yes)
                {
                    var employee = _unitOfWork.EmployeeRepo.Get(model.AccessDetails.EmployeeId);
                    model.AccessDetails.AccessCode = AccessCodeGenerator.NewCode(employee.StaffId);
                }
                _unitOfWork.AccessDetailsRepo.EditDetails(model.AccessDetails);
                await _unitOfWork.SaveAsync();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return PartialView("_EditUserAccess", model);
            }
        }

        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ManageUserAccess));
            }
            var model = _unitOfWork.AccessDetailsRepo.GetAccessDetails().SingleOrDefault(m => m.Id == id.Value);
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
                var accessDetails = _unitOfWork.AccessDetailsRepo.Get(id);
                _unitOfWork.AccessDetailsRepo.Remove(accessDetails);
                await _unitOfWork.SaveAsync();

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
            var employee = _unitOfWork.EmployeeRepo.GetEmployeeByMail(Email);
            if (employee == null)
                return Json("Employee with this email does not exists. Please enter a different email",  JsonRequestBehavior.AllowGet);

            var accessDetails = _unitOfWork.AccessDetailsRepo.GetByEmployeeId(employee.Id);
            return accessDetails == null? Json(true, JsonRequestBehavior.AllowGet) : Json("User already has an access level!", JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}