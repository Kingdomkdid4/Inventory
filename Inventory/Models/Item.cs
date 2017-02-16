using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Inventory.Models
{

    public class Item
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Amount { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }

        [Required]
        public int InventoryId { get; set; }

        public Inventory Inventory { get; set; }
       // public ICollection<Inventory> inventory { get; set; }
    }
}