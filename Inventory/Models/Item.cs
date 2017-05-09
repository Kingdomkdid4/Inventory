using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace InventoryManager.Models
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
        [Range(0, int.MaxValue)]
        public int InStock { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Compare("InStock", ErrorMessage = "The Amount In Stock Is Higher Then Your Total")]
        public int Total { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }

        
        //public string ExtraInfo { get; set; }

        [Required]
        public int InventoryId { get; set; }

        public Inventory Inventory { get; set; }

    }
}