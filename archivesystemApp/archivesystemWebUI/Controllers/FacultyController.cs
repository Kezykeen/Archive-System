using System.Threading.Tasks;
using System.Web.Mvc;
using archivesystemWebUI.Models;
using archivesystemWebUI.Services;

namespace archivesystemWebUI.Controllers
{
    //[Authorize(Roles = "Admin, Manager")]
    public class FacultyController : Controller
    {
        private readonly IFacultyService _facultyService;
        
        public FacultyController(IFacultyService facultyService)
        {
            _facultyService = facultyService;
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET json data for DataTable
        public ActionResult GetFacultyData()
        {
            var facultyData = _facultyService.GetFacultyData();

            return Json(new {data = facultyData}, JsonRequestBehavior.AllowGet);
        }

        //GET: Faculty/AddOrEdit/5?
        public ActionResult GetFacultyPartialView(int? id)
        {
            var model = _facultyService.GetFacultyViewModel(id);

            return PartialView("_AddOrEditFaculty", model);
        }

        [HttpPost]
        // POST: Faculty/AddOrEdit
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrEdit(FacultyViewModel model)
        {
            await _facultyService.AddOrEdit(model);

            return Json(new {success = true}, JsonRequestBehavior.AllowGet);
        }

        //GET: Faculty/Delete/5
        public ActionResult GetDeletePartialView(int id)
        {
            var faculty = _facultyService.GetFacultyById(id);
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
            await _facultyService.Delete(id);

            return Json(new {success = true}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FacultyNameCheck(string name, int id)
        {
            var status = _facultyService.FacultyNameCheck(name, id);

            return Json(!status, JsonRequestBehavior.AllowGet);
        }
    }
}
