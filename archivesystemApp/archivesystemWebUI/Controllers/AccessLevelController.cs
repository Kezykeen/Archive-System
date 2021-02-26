using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;
using System;
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
            //try
            //{
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
            //}
            //catch (Exception e)
            //{
            //ModelState.AddModelError("", e.Message);
            //return View(model);
            //}
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

        #endregion
    }
}