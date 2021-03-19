using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using archivesystemWebUI.Services;
using AutoMapper;

namespace archivesystemWebUI.Controllers
{
    //[Authorize(Roles = "Admin, Manager")]
    public class FacultyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IFacultyService _service;
        
        public FacultyController(IUnitOfWork unitOfWork, IFacultyService _service)
        {
            _unitOfWork = unitOfWork;
            this._service = _service;
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET json data for DataTable
        public ActionResult GetFacultyData()
        {
            var faculty = _unitOfWork.FacultyRepo.GetAllToList();
            var viewModel = faculty.Select(x => new {x.Name, x.Id});

            return Json(new {data = viewModel}, JsonRequestBehavior.AllowGet);
        }

        //GET: Faculty/AddOrEdit/5?
        public ActionResult GetFacultyPartialView(int? id)
        {
            var faculty = id != null ? _unitOfWork.FacultyRepo.Get(id.Value) : new Faculty();
            var model = Mapper.Map<FacultyViewModel>(faculty);

            return PartialView("_AddOrEditFaculty", model);
        }

        [HttpPost]
        // POST: Faculty/AddOrEdit
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrEdit(FacultyViewModel model)
        {
            if (model.Id == 0)
            {
                var result =_service.SaveFaculty(model);
                if(result== FacultyServiceResult.Succeeded)
                    return Json("success", JsonRequestBehavior.AllowGet);
                return Json("failure", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var getFaculty = _unitOfWork.FacultyRepo.Get(model.Id);
                Mapper.Map(model, getFaculty);
                getFaculty.UpdatedAt = DateTime.Now;

                _unitOfWork.FacultyRepo.Update(getFaculty);
                var folder = Mapper.Map<Folder>(model);
                _unitOfWork.FolderRepo.UpdateFacultyFolder(folder);
            }
            await _unitOfWork.SaveAsync();

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        //GET: Faculty/Delete/5
        public ActionResult GetDeletePartialView(int id)
        {
            Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            return PartialView("_DeleteFaculty", faculty);
        }

        // POST: Faculty/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Faculty faculty = _unitOfWork.FacultyRepo.Get(id);
                _unitOfWork.FacultyRepo.Remove(faculty);
                await _unitOfWork.SaveAsync();

                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("failure", JsonRequestBehavior.AllowGet);
            }
        }

       

        [HttpPost]
        public JsonResult FacultyNameCheck(string name, int id)
        {
            var faculties = _unitOfWork.FacultyRepo.GetAllToList();

            // Check if the entry name exists & change is from a different entry and return error message from viewModel
            bool status = faculties.Any(x => x.Name == name && x.Id != id);
            return Json(!status, JsonRequestBehavior.AllowGet);
        }
    }
}
