using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using AutoMapper;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace archivesystemWebUI.Controllers
{
    public class AccessLevelController : Controller
    {
        #region FIELDS
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region CONSTRUCTOR
        public AccessLevelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region ACTION METHODS

        #region AccessLevel
        /// <summary>
        /// This region contains all the actions to manage creation and manipulation of Access Level
        /// 
        /// These actions make use of AccessLevel table
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageAccessLevel()
        {
            var model = _unitOfWork.AccessLevelRepo.GetAll();
            return View(model);
        }

        public ActionResult CreateAccessLevel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAccessLevel(CreateAccessLevelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var checkLevel = _unitOfWork.AccessLevelRepo.GetByLevel(model.Level);
                if (checkLevel == null)
                {
                    var newAccess = Mapper.Map<AccessLevel>(model);
                    _unitOfWork.AccessLevelRepo.Add(newAccess);
                    await _unitOfWork.SaveAsync();
                    TempData["AccessMessage"] = $"Access Level was succesfully created!";
                    return RedirectToAction(nameof(ManageAccessLevel));
                }
                ModelState.AddModelError("AccessLevelExists", $"Access Level \"{model.Level}\" already exists. Please enter a different Level");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
            return View(model);
        }

        public ActionResult EditAccessLevel(int id)
        {
            var accessLevel = _unitOfWork.AccessLevelRepo.Get(id);
            if (accessLevel == null)
                return HttpNotFound();
            return View(accessLevel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAccessLevel(AccessLevel accessLevel)
        {
            if (!ModelState.IsValid)
            {
                return View(accessLevel);
            }
            try
            {
                accessLevel.UpdatedAt = DateTime.Now;
                _unitOfWork.AccessLevelRepo.EditDetails(accessLevel);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(ManageAccessLevel));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(accessLevel);
            }
        }

        public ActionResult DeleteAccessLevel(int? id)
        {
            if (id != null)
            {
                if ((id > 0) && (id <= 5))
                {
                    TempData["DeleteMessage"] = "This is Access Level cannot be deleted";
                    return RedirectToAction(nameof(ManageAccessLevel));
                }
                var model = _unitOfWork.AccessLevelRepo.Get(id.Value);
                if (model == null)
                {
                    return HttpNotFound();
                }
                return View(model);
            }
            return RedirectToAction(nameof(ManageAccessLevel));
        }

        [HttpPost, ActionName(nameof(DeleteAccessLevel))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAccessLevelConfirmed(int id)
        {
            try
            {
                var accessLevel = _unitOfWork.AccessLevelRepo.Get(id);
                _unitOfWork.AccessLevelRepo.Remove(accessLevel);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(ManageAccessLevel));
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region UserAccess
        /// <summary>
        /// This region contains all the actions to manage User Access Level such as:
        /// 1. Adding user to an access level
        /// 2. Removing user from an access level
        /// 3. Activating user access
        /// 
        /// These actions make use of AccessDetails table
        /// </summary>
        public ActionResult ManageUserAccess()
        {
            var model = _unitOfWork.AccessDetailsRepo.GetAll();
            return View(model);
        }

        public ActionResult AddUserToAccess()
        {
            var accessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            var viewModel = new AddUserToAccessViewModel { AccessLevels = accessLevels };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUserToAccess(AddUserToAccessViewModel model)
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
                return RedirectToAction(nameof(ManageUserAccess));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                return View(model);
            }
        }

        public ActionResult EditUserAccess(int id)
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
        public async Task<ActionResult> EditUserAccess(EditUserViewModel model)
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

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ManageAccessLevel));
            }
            var model = _unitOfWork.AccessDetailsRepo.Get(id.Value);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var accessDetails = _unitOfWork.AccessDetailsRepo.Get(id);
                _unitOfWork.AccessDetailsRepo.Remove(accessDetails);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(ManageUserAccess));
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #endregion
    }
}