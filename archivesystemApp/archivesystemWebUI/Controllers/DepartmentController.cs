using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET json data for DataTable
        public ActionResult GetDepartmentData()
        {
            var department = _unitOfWork.DeptRepo.GetAllToList();
            var viewModel = department.Select(x => new {x.Name, Faculty = x.Faculty.Name, x.Id});

            return Json(new {data = viewModel}, JsonRequestBehavior.AllowGet);
        }

        //GET: Department/AddOrEdit/5?
        public ActionResult GetDepartmentPartialView(int? id)
        {
            var department = id != null ? _unitOfWork.DeptRepo.Get(id.Value) : new Department();
            var model = Mapper.Map<DepartmentViewModel>(department);

            ViewBag.FacultyId = new SelectList(_unitOfWork.FacultyRepo.GetAll(), "Id", "Name", model.FacultyId);
            return PartialView("_AddOrEditDepartment", model);
        }

        [HttpPost]
        // POST: Department/AddOrEdit
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrEdit(DepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.FacultyId = new SelectList(_unitOfWork.FacultyRepo.GetAll(), "Id", "Name", model.FacultyId);
                return Json("failure", JsonRequestBehavior.AllowGet);
            }

            // Check if the entry name exists & change is from a different entry and return custom message
            var allDepartments = _unitOfWork.DeptRepo.GetAllToList();
            if (allDepartments.Any(x=> x.Name == model.Name && x.Id != model.Id))
            {
                var message = "Name already exist";
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            var department = Mapper.Map<Department>(model);
            if (model.Id == 0)
            {
                department.CreatedAt = DateTime.Now;
                department.UpdatedAt = DateTime.Now;

                _unitOfWork.DeptRepo.Add(department);
                var departmentFolder=CreateDepartmentFolder(department);
                
            }
            else
            {
                var getDepartment = _unitOfWork.DeptRepo.Get(model.Id);
                Mapper.Map(model, getDepartment);
                getDepartment.UpdatedAt = DateTime.Now;

                _unitOfWork.DeptRepo.Update(getDepartment);
            }
            await _unitOfWork.SaveAsync();

            ViewBag.FacultyId = new SelectList(_unitOfWork.FacultyRepo.GetAll(), "Id", "Name", model.FacultyId);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        //GET: Department/Delete/5
        public ActionResult GetDeletePartialView(int id)
        {
            Department department = _unitOfWork.DeptRepo.Get(id);
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
            try
            {
                Department department = _unitOfWork.DeptRepo.Get(id);
                _unitOfWork.DeptRepo.Remove(department);
                await _unitOfWork.SaveAsync();

                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("failure", JsonRequestBehavior.AllowGet);
            }
        }

        private Folder CreateDepartmentFolder(Department department)
        {
           
            var faculty = _unitOfWork.FacultyRepo.Get(department.FacultyId);
            var facultyFolder = _unitOfWork.FolderRepo.GetFacultyFolder(faculty.Name);
            var folder = new Folder
            {
                Name = department.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AccessLevelId = _unitOfWork.AccessLevelRepo.GetBaseLevel().Id,
                ParentId = facultyFolder.Id,
                IsRestricted=true
            };

            _unitOfWork.FolderRepo.Add(folder);
            return folder;

        }
    }
}
