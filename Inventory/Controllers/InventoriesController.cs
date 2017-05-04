using Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;

namespace Inventory.Controllers
{
    public class InventoriesController : Controller
    {
        private InventoryDbContext db = new InventoryDbContext();
        private ApplicationUserManager _userManager;

        public InventoriesController()
        {
        }

        public InventoriesController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Movies
        public ActionResult Index(string SearchString)
        {
            string currentUser = User.Identity.GetUserId();

            string currentUserEmail = User.Identity.GetUserName();

            var owned = db.Inventories.Where(s => s.UserId == currentUser);
            var shared = db.Inventories.Where(inv => inv.SharedUsers.Where(e => e.Email == currentUserEmail).Any());

            if (!String.IsNullOrEmpty(SearchString))
            {
                owned = owned.Where(s => s.Name.ToLower().Contains(SearchString.ToLower()));

                shared = shared.Where(s => s.Name.ToLower().Contains(SearchString.ToLower()));
            }

            return View(new { Owned = owned.ToList(), Shared = shared.ToList() });
        }

        public ActionResult Sharing(int inventoryId)
        {

            return View(new Sharing { InventoryId = inventoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sharing( Models.Sharing entry, string action, string permission)
        {
            string currentUser = User.Identity.GetUserId();

            bool ownedInventory = db.Inventories.Where(e => e.UserId == currentUser && e.Id == entry.InventoryId).Any();

            if (ModelState.IsValid && ownedInventory)
            {
                if (db.userDb.Where(s => s.Email == entry.Email) == null)
                {
                    //send email invitation
                }

                db.SharedUsers.Add(entry);
                db.SaveChanges();

                if (action == "ShareAndReturn")
                {
                    return RedirectToAction("Open", "Inventories", new { id = entry.InventoryId });
                }
                else
                {
                    return RedirectToAction("Sharing", "Inventories", new { id = entry.InventoryId });
                }
            }

            return View(entry);
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
        public ActionResult Create(/*[Bind(Include = "Id, Name", Exclude = "UserId")]*/ Models.Inventory entry, string action)
        {
            string currentUser = User.Identity.GetUserId();
            entry.UserId = User.Identity.GetUserId();
            ModelState.Clear();
            TryUpdateModel(entry);

            if (ModelState.IsValid)
            {
                db.Inventories.Add(entry);
                db.SaveChanges();

                if (action == "SaveAndOpen")
                {
                    return RedirectToAction("Open", "Inventories", new { id = entry.Id });
                }
                else
                {
                    return RedirectToAction("Index", "Inventories");
                }
            }
            return View(entry);
        }

        public ActionResult Open(int id, string searchString, string itemCategory, string itemType, string itemSize)
        {
            string currentUser = User.Identity.GetUserId();

            Models.Inventory inventory = db.Inventories.Include(inv => inv.Items).Where(inv => inv.Id == id && inv.UserId == currentUser).SingleOrDefault();
            
            var categories = inventory.Items.Select(e => e.Category).Distinct().OrderBy(e => e);
            ViewBag.itemCategory = new SelectList(categories);

            var types = inventory.Items.Select(e => e.Type).Distinct().OrderBy(e => e);

            ViewBag.itemType = new SelectList(types);

            var size = inventory.Items.Select(e => e.Size == null ? "N/A" : e.Size).Distinct().OrderBy(e => e);
            ViewBag.itemSize = new SelectList(size);

            if (inventory == null)
            {
                return HttpNotFound();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                inventory.Items = inventory.Items.Where(s => s.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(itemCategory))
            {
                inventory.Items = inventory.Items.Where(s => s.Category.Trim().Equals(itemCategory.Trim(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            if(!string.IsNullOrEmpty(itemType))
            {
                inventory.Items = inventory.Items.Where(s => s.Type.Trim().Equals(itemType.Trim(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(itemSize))
            {
                inventory.Items = inventory.Items.Where(s => itemSize == "N/A" && s.Size == null || s.Size.Trim().Equals(itemSize.Trim(), StringComparison.InvariantCultureIgnoreCase)).ToList();
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Models.Inventory inventory = db.Inventories.Find(id);
            db.Inventories.Remove(inventory);
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

        public ActionResult Edit(int? id)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include = "Name")]*/ Models.Inventory inventory, string action)
        {
            string currentUser = User.Identity.GetUserId();

            inventory.UserId = User.Identity.GetUserId();

            ModelState.Clear();

            TryUpdateModel(inventory);

            if (ModelState.IsValid)
            {
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();

                if (action == "SaveAndOpen")
                {
                    return RedirectToAction("Open", "Inventories", new { id = inventory.Id });
                }
                else
                {
                    return RedirectToAction("Index", "Inventories");
                }
            }

            return View(inventory);
        }
        
    }
}