using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public class Inventory
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        //public Item item { get; set; }
        public ICollection<Item> Items { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}