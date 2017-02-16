using Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Data.Entity;

namespace Inventory.Controllers
{
    public class InventoriesController : Controller
    {
        private InventoryDbContext db = new InventoryDbContext();

        // GET: Movies
        public ActionResult Index()
        {
            string currentUser = User.Identity.GetUserId();
            return View(db.Inventories.Where(e => e.UserId == currentUser).ToList());
        }

        public ActionResult Open()
        {
            string currentUser = User.Identity.GetUserId();
            return View(db.Inventories.Where(e => e.UserId == currentUser).ToList());
        }

        public ActionResult SearchIndex(string Id)
        {
            string currentUser = User.Identity.GetUserId();

            string searchString = Id;

            var inventories = from m in db.Inventories
                              select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                inventories = inventories.Where(s => s.Name.Contains(searchString));
            }

            return View(db.Inventories.Where(e => e.UserId == currentUser).ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Name", Exclude = "UserId")] Models.Inventory entry)
        {
            string currentUser = User.Identity.GetUserId();
            entry.UserId = User.Identity.GetUserId();
            ModelState.Clear();
            TryUpdateModel(entry);
            if (ModelState.IsValid)
            {
                db.Inventories.Add(entry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(entry);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Inventory inventory = db.Inventories.Find(id);
            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Models.Inventory movie = db.Inventories.Find(id);
            db.Inventories.Remove(movie);
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