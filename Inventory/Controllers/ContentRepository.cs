using InventoryManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace InventoryManager.Controllers
{
    public class ContentRepository
    {
        private InventoryDbContext db = new InventoryDbContext();
        public int UploadImageInDataBase(HttpPostedFileBase file, Item item)
        {
            item.Image = ConvertToBytes(file);
            var Content = new Item
            {
                Description = item.Description,
                Image = item.Image
            };

            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();

            int i = db.SaveChanges();
            if (i == 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
    }
}