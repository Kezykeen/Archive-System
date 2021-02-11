using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

        // GET: AccessLevel
        public ActionResult Index()
        {
            var model = _unitOfWork.AccessLevelRepo.AccessLevels;
            return View(model);
        }

        // GET: AccessLevel/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AccessLevel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccessLevel/Create
        [HttpPost]
        public async Task<ActionResult> Create(CreateAccessLevelViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                var newAccess = new AccessLevel { Level = model.Level, LevelName = model.LevelName, CreatedAt = DateTime.Now };
                _unitOfWork.AccessLevelRepo.CreateAccess(newAccess);
                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(model);
            }
        }

        // GET: AccessLevel/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccessLevel/Edit/5
        [HttpPost]
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
            return View();
        }

        // POST: AccessLevel/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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
