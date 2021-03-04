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
            if (!ModelState.IsValid)
            {
                return Json("failure", JsonRequestBehavior.AllowGet);
            }

            // Check if the entry name exists & change is from a different entry and return custom message
            var faculties = _unitOfWork.FacultyRepo.GetAllToList();
            if (faculties.Any(x => x.Name == model.Name && x.Id != model.Id))
            {
                var message = "Name already exist";
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            var faculty = Mapper.Map<Faculty>(model);
            if (model.Id == 0)
            {
                faculty.CreatedAt = DateTime.Now;
                faculty.UpdatedAt = DateTime.Now;
                _unitOfWork.FacultyRepo.Add(faculty);
                
               int folderId= CreateFacultyFolder(faculty);
               _unitOfWork.FolderRepo.AddFolderPath(folderId);
            }
            else
            {
                var getFaculty = _unitOfWork.FacultyRepo.Get(model.Id);
                Mapper.Map(model, getFaculty);
                getFaculty.UpdatedAt = DateTime.Now;

                _unitOfWork.FacultyRepo.Update(getFaculty);
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

        private  int CreateFacultyFolder(Faculty faculty)
        {
           
            var rootFolder = _unitOfWork.FolderRepo.GetRootFolder();
            var folder = new Folder
            {
                Name = faculty.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                AccessLevelId = _unitOfWork.AccessLevelRepo.GetBaseLevel().Id,
                ParentId=rootFolder.Id            
            };
            _unitOfWork.FolderRepo.Add(folder);
            _unitOfWork.Save();

            return folder.Id;
        }
    }
}
