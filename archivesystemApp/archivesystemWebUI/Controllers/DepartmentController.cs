using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.DataLayers;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{

    [Authorize(Roles = "Admin, Manager")]
    public class DepartmentController : Controller
    {
        #region Fields

        private readonly IDepartmentService _departmentService;
        #endregion

        #region Constructors

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        #endregion

        #region ActionMethods

        public ActionResult Index()
        {
            return View();
        }

        // GET json data for DataTable
        public ActionResult GetDepartmentData()
        {
            var model = _departmentService.GetAllDepartmentToList();
            var viewModel = model.Select(x => new {x.Name, Faculty = x.Faculty.Name, x.Id});

            return Json(new {data = viewModel}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartmentPartialView(int? id)
        {
            var model = _departmentService.GetDepartment(id);
            model.Faculties = _departmentService.GetAllFaculties();

            return PartialView("_AddOrEditDepartment", model);
        }

        [HttpPost]
        // POST: Department/AddOrUpdate
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdate(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AddOrEditDepartment", model);

            ServiceResult result;
            if (model.Id == 0)
            {
                var department = Mapper.Map<Department>(model);
                result = _departmentService.SaveDepartment(department);
            }
            else
            {
                var departmentInDb = _departmentService.GetDepartmentInDb(model.Id);
                Mapper.Map(model, departmentInDb);
                result = await _departmentService.UpdateDepartment(departmentInDb);
            }
            
            return result == ServiceResult.Succeeded
                ? Json(new { success = true }, JsonRequestBehavior.AllowGet)
                : Json(new { failure = true }, JsonRequestBehavior.AllowGet);
        }

        //GET: Department/Delete/5
        public ActionResult GetDeletePartialView(int id)
        {
            Department department = _departmentService.GetDepartmentById(id);
            if (department == null)
                return HttpNotFound();

            return PartialView("_DeleteDepartment", department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _departmentService.DeleteDepartment(id);
                return result == ServiceResult.Prohibited
                    ? Json(new {prohibited = true}, JsonRequestBehavior.AllowGet)
                    : Json(new {success = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new {failure = true}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewAllUsersInDept(int id)
        {
            var users = _departmentService.GetAllUsersInDepartment(id);
            var response = Mapper.Map<List<UserDataView>>(users);

            return View(response);
        }
        #endregion
        
        #region Validators

        //Remote validation for duplicate names
        [HttpPost]
        public JsonResult DepartmentNameCheck(string name, int id)
        {
            var status = _departmentService.DepartNameCheck(name, id);

            return Json(!status, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
