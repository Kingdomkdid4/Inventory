using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class InventoryDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
    }
}