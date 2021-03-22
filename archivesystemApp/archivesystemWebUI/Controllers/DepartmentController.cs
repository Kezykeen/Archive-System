using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using archivesystemWebUI.Services;

namespace archivesystemWebUI.Controllers
{

    [Authorize(Roles = "Admin, Manager")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(DepartmentService departmentService, IUnitOfWork unitOfWork)
        {
            _departmentService = departmentService;
            _unitOfWork = unitOfWork;
        }

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
            var model = _departmentService.GetDepartmentViewModel(id);
            model.Faculties = _departmentService.GetAllFaculties();

            return PartialView("_AddOrEditDepartment", model);
        }

        [HttpPost]
        // POST: Department/AddOrEdit
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit(DepartmentViewModel model)
        {
            _departmentService.AddOrEdit(model);
            model.Faculties = _departmentService.GetAllFaculties();
           
            return Json(new {success = true}, JsonRequestBehavior.AllowGet);
        }

        //GET: Department/Delete/5
        public ActionResult GetDeletePartialView(int id)
        {
            Department department = _departmentService.GetDepartmentById(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            return PartialView("_DeleteDepartment", department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _departmentService.Delete(id);

            return Json(new {success = true}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DepartmentNameCheck(string name, int id)
        {
            var status = _departmentService.DepartNameCheck(name, id);

            return Json(!status, JsonRequestBehavior.AllowGet);
        }
    }
}
