using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Controllers
{
    public class AccessDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AccessDetails
        public async Task<ActionResult> Index()
        {
            return View(await db.AccessDetails.ToListAsync());
        }

        // GET: AccessDetails/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessDetails accessDetails = await db.AccessDetails.FindAsync(id);
            if (accessDetails == null)
            {
                return HttpNotFound();
            }
            return View(accessDetails);
        }

        // GET: AccessDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccessDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,EmployeeId,EmployeeName,AccessLevel,AccessCode,Status")] AccessDetails accessDetails)
        {
            if (ModelState.IsValid)
            {
                db.AccessDetails.Add(accessDetails);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(accessDetails);
        }

        // GET: AccessDetails/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessDetails accessDetails = await db.AccessDetails.FindAsync(id);
            if (accessDetails == null)
            {
                return HttpNotFound();
            }
            return View(accessDetails);
        }

        // POST: AccessDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EmployeeId,EmployeeName,AccessLevel,AccessCode,Status")] AccessDetails accessDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accessDetails).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(accessDetails);
        }

        // GET: AccessDetails/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessDetails accessDetails = await db.AccessDetails.FindAsync(id);
            if (accessDetails == null)
            {
                return HttpNotFound();
            }
            return View(accessDetails);
        }

        // POST: AccessDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AccessDetails accessDetails = await db.AccessDetails.FindAsync(id);
            db.AccessDetails.Remove(accessDetails);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
