using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
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
        private readonly IAccessLevelService _service;
        #endregion

        #region CONSTRUCTOR
        public AccessLevelController( IAccessLevelService service)
        {
            _service = service;
        }

       
        #endregion

        #region ACTION METHODS
        /// <summary>
        /// This region contains all the actions to manage creation and manipulation of Access Level
        /// 
        /// These actions make use of "AccessLevel" table
        /// </summary>

        [Route("AccessLevels")]
        public ActionResult ManageAccessLevel()
        {
            var model = _service.GetAll();
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
                await _service.Create(newAccess);

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
            var accessLevel = _service.GetById(id);
            if (accessLevel == null)
                return HttpNotFound();

            var model = Mapper.Map<EditAccessLevelViewModel>(accessLevel);
            return PartialView("_EditAccessLevel", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAccessLevel(EditAccessLevelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditAccessLevel", model);
            }

            try
            {
                var accessLevel = Mapper.Map<AccessLevel>(model);
                await _service.Update(accessLevel);

                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return PartialView("_EditAccessLevel", model);
            }
        }

        #endregion

        #region Validators
        [HttpPost]
        public JsonResult LevelAvailable(string Level)
        {
            var status = _service.CheckLevel(Level);
            return Json(status);
        }
        #endregion 
    }
}