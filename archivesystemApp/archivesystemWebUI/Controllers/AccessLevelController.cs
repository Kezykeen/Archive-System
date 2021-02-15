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

        // GET: AccessLevel
        public ActionResult Index() 
        {
            var model = _unitOfWork.AccessLevelRepo.GetAll();
            return View(model);
        }

        // GET: AccessLevel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccessLevel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAccessLevelViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkAccess = _unitOfWork.AccessLevelRepo.GetByLevel(model.Level);
                    if (checkAccess == null)
                    {
                        //var newAccess = new AccessLevel { Level = model.Level, LevelDescription = model.LevelDescription, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
                        var newAccess = new AccessLevel();
                        Mapper.Map(model, newAccess);
                        _unitOfWork.AccessLevelRepo.Add(newAccess);
                        await _unitOfWork.SaveAsync();
                        return RedirectToAction("Index");
                    }

                    ModelState.AddModelError("AccessLevelExists", $"Access Level \"{model.Level}\" already exists. Please enter a different Level");

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    return View(model);
                }
            }
            return View(model);
        }
                
        public ActionResult AddUserToAccess()
        {
            var accessLevels = _unitOfWork.AccessLevelRepo.GetAll();
            var viewModel = new AddUserToAccessViewModel
            {
                AccessLevels = accessLevels
            };
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
                    var employee = _unitOfWork.EmployeeRepo.GetEmployee(model.UserIdentification);
                    if (employee != null)
                    {
                        var accessCode = employee.Name.Substring(0, 2) + DateTime.Now.Day.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Hour.ToString() + model.AccessLevel;
                        var newUserAccess = new AccessDetails { EmployeeId = employee.Id, EmployeeName = employee.Name, AccessLevel = model.AccessLevel, AccessCode = accessCode, Status = Status.Active };
                        //Mapper.Map(model, newUserAccess);
                        _unitOfWork.AccessDetailsRepo.Add(newUserAccess);
                        await _unitOfWork.SaveAsync();
                        return RedirectToAction(nameof(ManageUserAccess));
                    }

                    ModelState.AddModelError("EmployeeDoesNotExist", $"Employee with email \"{model.UserIdentification}\" does not exists. Please enter a different email");

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    return View(model);
                }
            }
            return View(model);

        }




        public ActionResult ManageUserAccess()
        {
            var model = _unitOfWork.AccessDetailsRepo.GetAll();
            return View(model);
        }

        // GET: AccessLevel/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // GET: AccessLevel/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccessLevel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AccessLevel/Delete/5
        public ActionResult Delete(int id)
        {          
            var model = _unitOfWork.AccessDetailsRepo.Get(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        // POST: AccessLevel/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, AccessDetails model)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
        #endregion
}
