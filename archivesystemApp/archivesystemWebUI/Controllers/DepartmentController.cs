using System;
using System.Collections.Generic;
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

        // GET: Department
        public ActionResult Index()
        {
            var departments = _unitOfWork.DeptRepo.GetDeptWithFaculty();
            List<DepartmentViewModel> departmentViewModel = departments.Select(x=>new DepartmentViewModel
            {
                FacultyId = x.FacultyId,
                Name =  x.Name,
                Id =  x.Id
            }).ToList();

            ViewBag.DepartmentViewModel = departmentViewModel;
            return View(departments);
        }

        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            Department department = _unitOfWork.DeptRepo.Get(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            ViewBag.FacultyId = new SelectList(_unitOfWork.FacultyRepo.GetAll(), "Id", "Name");
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                Department department = Mapper.Map<Department>(model);
                department.CreatedAt = DateTime.Now;
                department.UpdatedAt = DateTime.Now;

                _unitOfWork.DeptRepo.Add(department);
                await _unitOfWork.SaveAsync();
                
                return View("Index");
            }

            ViewBag.FacultyId = new SelectList(_unitOfWork.FacultyRepo.GetAll(), "Id", "Name", model.FacultyId);
            return View(model);
        }

        // GET: Department/Edit/5
        public ActionResult Edit(int id)
        {
            Department department = _unitOfWork.DeptRepo.Get(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.FacultyId = new SelectList(_unitOfWork.FacultyRepo.GetAll(), "Id", "Name", department.FacultyId);

            DepartmentViewModel model = Mapper.Map<DepartmentViewModel>(department);

            return View(model);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                Department getDepartment = _unitOfWork.DeptRepo.Get(model.Id);
                var department = Mapper.Map(model, getDepartment);
                department.UpdatedAt = DateTime.Now;

                _unitOfWork.DeptRepo.Update(department);
                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index");
            }
            ViewBag.FacultyId = new SelectList(_unitOfWork.FacultyRepo.GetAll(), "Id", "Name", model.FacultyId);
            return View(model);
        }

        public ActionResult GetDepartmentData()
        {
            var departments = _unitOfWork.DeptRepo.GetAllToList();
            var viewModel = departments.Select(x => new {Name = x.Name, Faculty = x.Faculty.Name, Id = x.Id});

            return Json(new {data = viewModel}, JsonRequestBehavior.AllowGet);
        }

        // GET: Department/Delete/5
        public ActionResult Delete(int id)
        {
            Department department = _unitOfWork.DeptRepo.Get(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Department department = _unitOfWork.DeptRepo.Get(id);
            _unitOfWork.DeptRepo.Remove(department);
            await _unitOfWork.SaveAsync();
            
            return RedirectToAction("Index");
        }


    }
}
