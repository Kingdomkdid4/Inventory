using InventoryManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Mail;

namespace InventoryManager.Controllers
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

        public ActionResult Index(string SearchString)
        {
            string currentUser = User.Identity.GetUserId();

            string currentUserEmail = User.Identity.GetUserName();

            var owned = db.Inventories.Where(s => s.UserId == currentUser).Select(InventoryViewModel.GetInstantiator(currentUser, currentUserEmail));
            var shared = db.Inventories.Where(inv => inv.SharedUsers.Where(e => e.Email == currentUserEmail).Any()).Select(InventoryViewModel.GetInstantiator(currentUser, currentUserEmail));

            if (!String.IsNullOrEmpty(SearchString))
            {
                owned = owned.Where(s => s.Name.ToLower().Contains(SearchString.ToLower()));

                shared = shared.Where(s => s.Name.ToLower().Contains(SearchString.ToLower()));
            }

            return View(new InventoryListViewModel { Owned = owned.ToList(), Shared = shared.ToList() });
        }

        public ActionResult Shared(int inventoryId)
        {
            string currentUserEmail = User.Identity.GetUserName();

            IQueryable<Sharing> shared = db.SharedUsers.Where(e => e.InventoryId == inventoryId && e.Email != currentUserEmail).AsQueryable();

            return View(shared);
        }

        public ActionResult SharedEdit(int? id)
        {
            string currentUser = User.Identity.GetUserId();

            string currentUserEmail = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sharing entry = db.SharedUsers.Where(e => e.Id == id).SingleOrDefault();

            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SharedEdit(/*[Bind(Include = "Name")]*/ Sharing entry, string action)
        {
            string currentUser = User.Identity.GetUserId();

            string currentUserEmail = User.Identity.GetUserName();


            bool isAllowed = db.Inventories.Where(inv => inv.Id == entry.InventoryId && (inv.UserId == currentUser || inv.SharedUsers.Any(s => s.Email == currentUserEmail && (s.Permission == AccessLevel.Edit || s.Permission == AccessLevel.Admin)))).Any();

            if (ModelState.IsValid && isAllowed)
            {
                db.Entry(entry).State = EntityState.Modified;
                db.SaveChanges();

                if (action == "SaveAndOpen")
                {
                    return RedirectToAction("Open", "Inventories", new { id = entry.InventoryId });
                }
                else
                {
                    return RedirectToAction("Shared", "Inventories", new { inventoryid = entry.InventoryId });
                }
            }

            return View(entry);
        }

        public ActionResult SharedDelete(int? id)
        {
            string currentUser = User.Identity.GetUserId();

            string currentUserEmail = User.Identity.GetUserName();

            Sharing entry = db.SharedUsers.Where(e => e.Id == id).SingleOrDefault();

            bool isAllowed = db.Inventories.Where(inv => inv.Id == entry.InventoryId && (inv.UserId == currentUser || inv.SharedUsers.Any(s => s.Email == currentUserEmail && (s.Permission == AccessLevel.Edit || s.Permission == AccessLevel.Admin)))).Any();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (entry == null || !isAllowed)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        [HttpPost, ActionName("SharedDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult SharedDeleteConfirmed(int id)
        {
            Sharing entry = db.SharedUsers.Find(id);
            db.SharedUsers.Remove(entry);
            db.SaveChanges();
            return RedirectToAction("Open", "Inventories", new { id = entry.InventoryId });
        }

        public ActionResult Sharing(int inventoryId)
        {

            return View(new Sharing { InventoryId = inventoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sharing(Sharing entry, Invitations model, string action, string permission)
        {
            string currentUser = User.Identity.GetUserId();

            bool ownedInventory = db.Inventories.Where(e => e.UserId == currentUser && e.Id == entry.InventoryId).Any();

            //var user = UserManager.Users.Where(e => e.Id == currentUser);

            //UserManager.FindById<>
            if (ModelState.IsValid && ownedInventory)
            {
                if (!UserManager.Users.Where(s => s.Email == entry.Email).Any())
                { 
                    var body = $"<p>{model.Message}</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(entry.Email));
                    message.From = new MailAddress(model.FromEmail, model.FromName);  // replace with valid value
                    message.Subject = " Inventory has been shared with you!";
                    message.Body = body;
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "inventory.it.invitation@gmail.com", 
                            Password = "SecurePassword" 
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(message);
                    }
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

            string currentUserEmail = User.Identity.GetUserName();

            InventoryViewModel inventory = db.Inventories.Include(inv => inv.Items).Where(inv => inv.Id == id && (inv.UserId == currentUser || inv.SharedUsers.Any(s => s.Email == currentUserEmail))).Select(InventoryViewModel.GetInstantiator(currentUser, currentUserEmail)).SingleOrDefault();

            var categories = inventory.Items.Select(e => e.Category).Distinct().OrderBy(e => e);
            ViewBag.itemCategory = new SelectList(categories);

            var types = inventory.Items.Select(e => e.Type).Distinct().OrderBy(e => e);

            ViewBag.itemType = new SelectList(types);

            var size = inventory.Items.Select(e => e.Size == null ? "N/A" : e.Size).Distinct().OrderBy(e => e);
            ViewBag.itemSize = new SelectList(size);

            if (inventory == null || inventory.Permission == null)
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
            if (!string.IsNullOrEmpty(itemType))
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
            string currentUser = User.Identity.GetUserId();

            string currentUserEmail = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InventoryViewModel inventory = db.Inventories.Where(e => e.Id == id).Select(InventoryViewModel.GetInstantiator(currentUser, currentUserEmail)).SingleOrDefault();

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
            string currentUser = User.Identity.GetUserId();

            string currentUserEmail = User.Identity.GetUserName();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InventoryViewModel inventory = db.Inventories.Where(e => e.Id == id).Select(InventoryViewModel.GetInstantiator(currentUser, currentUserEmail)).SingleOrDefault();

            if (inventory == null)
            {
                return HttpNotFound();
            }
            return View(inventory);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(/*[Bind(Include = "Name")]*/ Inventory inventory, string action)
        {
            string currentUser = User.Identity.GetUserId();

            bool ownedInventory = db.Inventories.Where(e => e.UserId == currentUser && e.Id == inventory.Id).Any();

            if (ModelState.IsValid || ownedInventory)
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