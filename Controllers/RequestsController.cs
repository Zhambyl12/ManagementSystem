using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManagementSystem.Models;
using Microsoft.AspNet.Identity;

namespace ManagementSystem.Controllers
{
    public class RequestsController : Controller
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // GET: Requests
        public ActionResult Index()
        {
            return View(db.Requests.ToList());
        }

        // GET: Requests/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requests requests = db.Requests.Find(id);
            if (requests == null)
            {
                return HttpNotFound();
            }
            return View(requests);
        }

        // GET: Requests/Create
        public ActionResult Create()
        {
            db = new ApplicationDbContext();
            ViewBag.Users = db.Users.Select(x => new SelectListItem()
            {
                Selected = false,
                Text = x.LastName + " " + x.FirstName + " " + x.MiddleName,
                Value = x.Id.ToString()
            }).ToList();
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StartedDate,FinishedDate,UserId,ExecutorId,SignerId,SignedDate,DocName,DocUrl,ProcessType,Status")] Requests requests, HttpPostedFileBase File)
        { 
            if (ModelState.IsValid)
            {
                requests.Id = Guid.NewGuid();
                requests.StartedDate = DateTime.Now;
                requests.ExecutorId = new Guid(User.Identity.GetUserId());
                if (File != null && File.ContentLength > 0)
                    requests.DocUrl = Misc.SaveFileToUser(File, requests.UserId.ToString(), requests.ProcessType + requests.StartedDate.ToString("HH:mm:ss_dd.MM.yyyy"));
                else
                    goto next;
                requests.Status = "На рассмотрении";
                db.Requests.Add(requests);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            next:
            db = new ApplicationDbContext();
            ViewBag.Users = db.Users.Select(x => new SelectListItem()
            {
                Selected = false,
                Text = x.LastName + " " + x.FirstName + " " + x.MiddleName,
                Value = x.Id.ToString()
            }).ToList();
            return View(requests);
        }

        // GET: Requests/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requests requests = db.Requests.Find(id);
            if (requests == null)
            {
                return HttpNotFound();
            }
            return View(requests);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StartedDate,FinishedDate,UserId,ExecutorId,SignerId,SignedDate,DocName,DocUrl,ProcessType,Status")] Requests requests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(requests);
        }

        // GET: Requests/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Requests requests = db.Requests.Find(id);
            if (requests == null)
            {
                return HttpNotFound();
            }
            return View(requests);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Requests requests = db.Requests.Find(id);
            db.Requests.Remove(requests);
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
