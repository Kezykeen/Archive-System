using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace archivesystemWebUI.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
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
        /// <summary>
        /// This region contains all the actions to manage creation and manipulation of Access Level
        /// 
        /// These actions make use of "AccessLevel" table
        /// </summary>

        public ActionResult ManageAccessLevel()
        {
            var model = _unitOfWork.AccessLevelRepo.GetAll();
            return View(model);
        }
        
        public ActionResult CreateAccessLevel()
        {
            return PartialView("_AddAccessLevel");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAccessLevel(CreateAccessLevelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AddAccessLevel", model);
            }

            try
            {
                var newAccess = Mapper.Map<AccessLevel>(model);
                _unitOfWork.AccessLevelRepo.Add(newAccess);
                await _unitOfWork.SaveAsync();

                return Json("success", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return Json("failure", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditAccessLevel(int id)
        {
            var accessLevel = _unitOfWork.AccessLevelRepo.Get(id);
            if (accessLevel == null)
                return HttpNotFound();

            return PartialView("_EditAccessLevel", accessLevel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAccessLevel(AccessLevel accessLevel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditAccessLevel", accessLevel);
            }

            try
            {
                accessLevel.UpdatedAt = DateTime.Now;
                _unitOfWork.AccessLevelRepo.EditDetails(accessLevel);
                await _unitOfWork.SaveAsync();

                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return PartialView("_EditAccessLevel", accessLevel);
            }
        }

        #endregion

        #region Validators

        [HttpPost]
        public JsonResult LevelAvailable(string Level)
        {
            var checkLevel = _unitOfWork.AccessLevelRepo.GetByLevel(Level);
            bool status = checkLevel == null;
            return Json(status);
        }
        #endregion
      


    }
}
