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
    public class FacultyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        

        public FacultyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Faculties
        public ActionResult Index()
        {
            return View( _unitOfWork.FacultyRepo.GetAll());
        }


        // GET: Faculties/Details/5
        public ActionResult Details(int id)
        {
            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            return View(faculty);
        }

        // GET: Faculties/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Faculties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FacultyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var faculty = Mapper.Map<Faculty>(model);
                faculty.CreatedAt = DateTime.Now;
                faculty.UpdatedAt = DateTime.Now;

                _unitOfWork.FacultyRepo.Add(faculty);
                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Faculties/Edit/5
        public ActionResult Edit(int id)
        {
            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            var facultyViewModel = Mapper.Map<FacultyViewModel>(faculty);
            return View(facultyViewModel);
        }

        // POST: Faculties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FacultyViewModel model)
        {
            if (ModelState.IsValid)
            {
                Faculty getFaculty = _unitOfWork.FacultyRepo.Get(model.Id);
                var faculty = Mapper.Map(model, getFaculty);
                faculty.UpdatedAt = DateTime.Now;

                _unitOfWork.FacultyRepo.Update(faculty);
                await _unitOfWork.SaveAsync();

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult GetFacultyData()
        {
            var faculty = _unitOfWork.FacultyRepo.GetAllToList();
            var viewModel = faculty.Select(x => new {Name = x.Name, Id = x.Id});

            return Json(new {data = viewModel}, JsonRequestBehavior.AllowGet);
        }

        // GET: Faculties/Delete/5
        public ActionResult Delete(int id)
        {
            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            return View(faculty);
        }

        // POST: Faculties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
            if (faculty != null)
            {
                _unitOfWork.FacultyRepo.Remove(faculty);
                await _unitOfWork.SaveAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
