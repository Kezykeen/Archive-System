using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class FacultyController : Controller
    {
        #region Fields

        private readonly IFacultyService _facultyService;

        #endregion

        #region Constructors

        public FacultyController(IFacultyService facultyService)
        {
            _facultyService = facultyService;
        }
        #endregion

        #region ActionMethods

        public ActionResult Index()
        {
            return View();
        }

        // GET json data for DataTable
        public ActionResult GetFacultyData()
        {
            var facultyData = _facultyService.GetFacultyData();
            var map = Mapper.Map<IEnumerable<FacultyViewModel>>(facultyData);

            return Json(new {data = map}, JsonRequestBehavior.AllowGet);
        }

        //GET: Faculty/AddOrUpdate/5?
        public ActionResult GetFacultyPartialView(int? id)
        {
            var faculty = _facultyService.GetFaculty(id);
            var model = Mapper.Map<FacultyViewModel>(faculty);

            return PartialView("_AddOrEditFaculty", model);
        }

        [HttpPost]
        // POST: Faculty/AddOrUpdate
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdate(FacultyViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AddOrEditFaculty", model);

            ServiceResult result;
            if (model.Id == 0)
            {
                var faculty = Mapper.Map<Faculty>(model);
                result = _facultyService.SaveFaculty(faculty);
            }
            else
            {
                var facultyInDb = _facultyService.GetFacultyInDb(model.Id);
                Mapper.Map(model, facultyInDb);
                result = await _facultyService.UpdateFaculty(facultyInDb);
            }
            
            return result == ServiceResult.Succeeded 
                ? Json(new {success = true}, JsonRequestBehavior.AllowGet) 
                : Json(new {failure = true}, JsonRequestBehavior.AllowGet);
        }

        //GET: Faculty/Delete/5
        public ActionResult GetDeletePartialView(int id)
        {
            var faculty = _facultyService.GetFacultyById(id);
            if (faculty == null)
                return HttpNotFound();

            return PartialView("_DeleteFaculty", faculty);
        }

        // POST: Faculty/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _facultyService.DeleteFaculty(id);
                return result == ServiceResult.Prohibited 
                    ? Json(new {prohibited = true}, JsonRequestBehavior.AllowGet) 
                    : Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new {failure = true}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewAllDepartmentsInFaculty(int id)
        {
            var departments = _facultyService.GetAllDepartmentsInFaculty(id);

            return View(departments);
        }
        #endregion

        #region Validators
        //Remote validation for duplicate names
        [HttpPost]
        public JsonResult FacultyNameCheck(string name, int id)
        {
            var status = _facultyService.FacultyNameCheck(name, id);

            return Json(!status, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
