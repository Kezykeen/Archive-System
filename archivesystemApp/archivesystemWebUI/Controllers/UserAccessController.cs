using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using System;
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
            var model = _unitOfWork.AccessDetailsRepo.GetAll();
            return View(model);
        }

        public ActionResult AddUser()
        {
            var accessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            var viewModel = new AddUserToAccessViewModel { AccessLevels = accessLevels };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(AddUserToAccessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return View(model);
            }

            try
            {
                var employee = _unitOfWork.EmployeeRepo.GetEmployeeByMail(model.Email);
                if (employee == null)
                {
                    ModelState.AddModelError("EmployeeDoesNotExist", $"Employee with email \"{model.Email}\" does not exists. Please enter a different email");
                    model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                    return View(model);
                }
                var checkAccess = _unitOfWork.AccessDetailsRepo.GetByEmployeeId(employee.Id);
                if (checkAccess != null)
                {
                    TempData["UserAccessMessage"] = "User already has an access level!";
                    return RedirectToAction(nameof(ManageUserAccess));
                }
                var newAccessDetails = new AccessDetails
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.Name,
                    AccessLevel = model.AccessLevel,
                    AccessCode = AccessCodeGenerator.NewCode(employee.StaffId),
                    Status = Status.Active
                };
                _unitOfWork.AccessDetailsRepo.Add(newAccessDetails);
                await _unitOfWork.SaveAsync();
                TempData["UserAddedMessage"] = "Successfully added user to an access level";
                return RedirectToAction(nameof(ManageUserAccess));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return View(model);
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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return View(model);
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
                return RedirectToAction(nameof(ManageUserAccess));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return View(model);
            }
        }

        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ManageUserAccess));
            }
            var model = _unitOfWork.AccessDetailsRepo.Get(id.Value);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
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
                TempData["UserDeletedMessage"] = "User has been removed!";

                return RedirectToAction(nameof(ManageUserAccess));
            }
            catch
            {
                return View();
            }
        }
        #endregion
    }
}