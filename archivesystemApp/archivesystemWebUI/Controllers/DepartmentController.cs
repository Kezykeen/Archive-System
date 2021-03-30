using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{

    //[Authorize(Roles = "Admin, Manager")]
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

        public ActionResult GetDepartmentPartialView(int id)
        {
            var model = _departmentService.GetDepartmentForPartialView(id);
            var viewModel = Mapper.Map<DepartmentViewModel>(model);

            viewModel.Faculties = _departmentService.GetAllFaculties();

            return PartialView("_AddOrEditDepartment", viewModel);
        }

        [HttpPost]
        // POST: Department/AddOrUpdate
        //[ValidateAntiForgeryToken]
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
                var departmentInDb = _departmentService.GetDepartmentById(model.Id);
                departmentInDb.UpdatedAt = DateTime.Now;
                Mapper.Map(model, departmentInDb);
                result = _departmentService.UpdateDepartment(departmentInDb);
                UpdateDepartmentFolder(departmentInDb, result);
                await _departmentService.SaveChanges();
            }

            return result == ServiceResult.Succeeded
                ? Json(new { success = true }, JsonRequestBehavior.AllowGet)
                : Json(new { failure = true }, JsonRequestBehavior.AllowGet);
        }

        public void UpdateDepartmentFolder(Department department, ServiceResult result)
        {
            if (result != ServiceResult.Succeeded) return;
            var folder = Mapper.Map<Folder>(department);
            _departmentService.UpdateDepartmentFolder(folder);
        }

        //GET: Department/Delete/5
        public ActionResult GetDeletePartialView(int id)
        {
            var department = _departmentService.GetDepartmentById(id);
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
                    ? Json(new {prohibited = true, errorMessage = "Delete prohibited, kindly empty the user column"}, JsonRequestBehavior.AllowGet)
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

            return View(users);
        }
        #endregion
        
        #region Validators

        //Remote validation for duplicate names
        [HttpPost]
        public JsonResult DepartmentNameCheck(string name, int id)
        {
            var status = _departmentService.DepartmentNameCheck(name, id);

            return Json(!status, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
