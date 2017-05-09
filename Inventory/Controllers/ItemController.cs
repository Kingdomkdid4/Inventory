using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventoryManager.Models;
using System.Net;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.IO;

namespace InventoryManager.Controllers
{
    public class ItemController : Controller
    {
        private InventoryDbContext db = new InventoryDbContext();

        public ActionResult Create(int inventoryId)
        {
            Models.Inventory inventory = db.Inventories.Find(inventoryId);

            return View(new Item { InventoryId = inventoryId, Name = inventory.DefaultName, Category = inventory.DefaultCategory, Type = inventory.DefaultType, InStock = inventory.DefaultInStock, Total = inventory.DefaultTotal, Size = inventory.DefaultSize, Color = inventory.DefaultColor });
        }

        
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Create(/*[Bind(Include = "Id,Name,Category,Type,Amount,Size, Color, InventoryId")]*/ Models.Item item, string action)
         {
            string currentUser = User.Identity.GetUserId();

            item.Size = string.IsNullOrWhiteSpace(item.Size) ? null : item.Size.Trim();

            bool isAllowed = db.Inventories.Where(e => e.UserId == currentUser && e.Id == item.InventoryId).Any();

            if (ModelState.IsValid && isAllowed)
             {
                 db.Items.Add(item);
                 db.SaveChanges();

                if (action == "SaveAndOpen")
                {
                    return RedirectToAction("Open", "Inventories", new { id = item.InventoryId });
                }
                else
                {
                    return RedirectToAction("Create", "Item", new { inventoryId = item.InventoryId });
                }
            }

            return View(item);
         }

        public ActionResult Edit(int? id)
        {

            if (id == null)
         {
             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
         }
            Item item = db.Items.Find(id);
            string currentUser = User.Identity.GetUserId();
            bool isAllowed = db.Inventories.Where(e => e.UserId == currentUser && e.Id == item.InventoryId).Any();

            if (item == null || !isAllowed)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include = "Name,Category,Type,Amount,Size,Color, InventoryId")]*/ Models.Item item)
        {
            string currentUser = User.Identity.GetUserId();
            item.Size = string.IsNullOrWhiteSpace(item.Size) ? null : item.Size.Trim();

            bool isAllowed = db.Inventories.Where(e => e.UserId == currentUser && e.Id == item.InventoryId).Any();

            if (ModelState.IsValid && isAllowed)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Open", "Inventories", new { id = item.InventoryId });
            }
            return View(item);
        }
        public ActionResult Details(int id)
        {
            Item item = db.Items.Find(id);

            string currentUser = User.Identity.GetUserId();
            //bool isAllowed = db.Inventories.Where(e => e.UserId == currentUser && item.Id == picId).Any();

            return View(item);
        }

        public ActionResult Details(HttpPostedFileBase file)
        {
            //Models.Item item;

            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/temp"/* + item.Id + "/" + item.PicId*/));
                // file is uploaded
                file.SaveAs(path);

                // save the image path path to the database or you can send image
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

            }
            // after successfully uploading redirect the user
            return RedirectToAction("Details", "Item");
        }

        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Open", "Inventories", new { id = item.InventoryId });
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