using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using System.Net;

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
        public ActionResult Index() 
        {
            return View();
        }
        
        public ActionResult ManageAccessLevel() 
        {
            return View(_unitOfWork.AccessLevelRepo.GetAll());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccessLevelViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkLevel = _unitOfWork.AccessLevelRepo.GetByLevel(model.Level);
                    if (checkLevel == null)
                    {
                        var newAccess = new AccessLevel();
                        Mapper.Map(model, newAccess);
                        _unitOfWork.AccessLevelRepo.Add(newAccess);
                        await _unitOfWork.SaveAsync();
                        return RedirectToAction(nameof(Index));
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
            return View(model);
        }
        #endregion


        #region UserAccess
        public ActionResult ManageUserAccess()
        {
            var model = _unitOfWork.AccessDetailsRepo.GetAll();
            return View(model);
        }

        public ActionResult AddUserToAccess()
        {
            var accessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            var viewModel = new AddUserToAccessViewModel{ AccessLevels = accessLevels };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUserToAccess(AddUserToAccessViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = _unitOfWork.EmployeeRepo.GetEmployeeByMail(model.UserIdentification);
                    if (employee != null)
                    {
                        var accessCode = employee.Name.Substring(0, 2) + DateTime.Now.Day.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Hour.ToString() + model.AccessLevel;
                        var newAccessDetails = new AccessDetails { EmployeeId = employee.Id, EmployeeName = employee.Name, AccessLevel = model.AccessLevel, AccessCode = accessCode, Status = Status.Active };
                        //Mapper.Map(model, newAccessDetails);
                        _unitOfWork.AccessDetailsRepo.Add(newAccessDetails);
                        await _unitOfWork.SaveAsync();
                        return RedirectToAction(nameof(ManageUserAccess));
                    }
                    ModelState.AddModelError("EmployeeDoesNotExist", $"Employee with email \"{model.UserIdentification}\" does not exists. Please enter a different email");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
                    return View(model);
                }
            }
            model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            
            var accessDetails = _unitOfWork.AccessDetailsRepo.Get(id);
            if (accessDetails == null)
                return HttpNotFound();
            var accessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            var model = new EditUserviewModel
            {
                RegenerateCode = CodeStatus.No,
                AccessDetails = accessDetails,
                AccessLevels = accessLevels
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserviewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.RegenerateCode == CodeStatus.Yes)
                    {
                        var accessCode = model.AccessDetails.EmployeeName.Substring(0, 2) + DateTime.Now.Day.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Hour.ToString() + model.AccessDetails.AccessLevel;
                        model.AccessDetails.AccessCode = accessCode;
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
            model.AccessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            return View(model);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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