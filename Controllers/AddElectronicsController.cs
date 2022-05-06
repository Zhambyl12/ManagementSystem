using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManagementSystem.Models;

namespace ManagementSystem.Controllers
{
    public class AddElectronicsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AddElectronics
        public ActionResult Index()
        {
            return View(db.Electronics.ToList());
        }

        // GET: AddElectronics/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Electronics electronics = db.Electronics.Find(id);
            if (electronics == null)
            {
                return HttpNotFound();
            }
            return View(electronics);
        }

        // GET: AddElectronics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddElectronics/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SerialNumber,Title,CPU,RAM,DiskSize,DiskType,Video,OS,UserId")] Electronics electronics)
        {
            if (ModelState.IsValid)
            {
                electronics.Id = Guid.NewGuid();
                db.Electronics.Add(electronics);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(electronics);
        }

        // GET: AddElectronics/Edit/5
        public ActionResult Edit(Guid? id)
        {
            ViewBag.Users = db.Users.Select(x => new SelectListItem()
            {
                Selected = false,
                Text = x.LastName + " " + x.FirstName + " "+x.MiddleName,
                Value = x.Id.ToString()
            }).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Electronics electronics = db.Electronics.Find(id);
            if (electronics == null)
            {
                return HttpNotFound();
            }
            return View(electronics);
        }

        // POST: AddElectronics/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SerialNumber,Title,CPU,RAM,DiskSize,DiskType,Video,OS,UserId")] Electronics electronics)
        {
            if (ModelState.IsValid)
            {
                db.Entry(electronics).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(electronics);
        }

        // GET: AddElectronics/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Electronics electronics = db.Electronics.Find(id);
            if (electronics == null)
            {
                return HttpNotFound();
            }
            return View(electronics);
        }

        // POST: AddElectronics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Electronics electronics = db.Electronics.Find(id);
            db.Electronics.Remove(electronics);
            db.SaveChanges();
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
