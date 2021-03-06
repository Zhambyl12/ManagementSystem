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

    public class ElectronicsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "IT-spec")]
        public ActionResult Balanced()
        {
            return View(Misc.GetAllElectronics());
        }
        [Authorize(Roles = "IT-spec")]
        public ActionResult Ordered()
        {
            return View(Misc.GetAllElectronics());
        }
        [Authorize(Roles ="IT-spec, HR-manager")]
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
        [Authorize(Roles = "IT-spec,HR-manager")]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "IT-spec,HR-manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SerialNumber,Title,CPU,RAM,DiskSize,DiskType,Video,OS")] Electronics electronics)
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
        [Authorize(Roles = "IT-spec")]
        public ActionResult Edit(Guid? id)
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
        [Authorize(Roles = "IT-spec")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SerialNumber,Title,CPU,RAM,DiskSize,DiskType,Video,OS")] Electronics electronics)
        {
            if (ModelState.IsValid)
            {
                db.Entry(electronics).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(electronics);
        }
        [Authorize(Roles = "IT-spec")]
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
        [Authorize(Roles = "IT-spec")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Electronics electronics = db.Electronics.Find(id);
            db.Electronics.Remove(electronics);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "IT-spec")]
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
