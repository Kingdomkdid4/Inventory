using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManager.Models
{
    public class Inventory
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string DefaultName { get; set; }

        public string DefaultCategory { get; set; }

        public string DefaultType { get; set; }

        public int DefaultInStock { get; set; }

        public int DefaultTotal { get; set; }

        public string DefaultSize { get; set; }

        public string DefaultColor { get; set; }

        //public Item item { get; set; }

        //public List<string> SharedEmailsWithFullAccess  = new List<string>();

        //public List<string> SharedEmailsWithAddingAccess  = new List<string>();

        //public List<string> SharedEmailsWithViewAccess  = new List<string>();

        public ICollection<Item> Items { get; set; }

        public ICollection<Sharing> SharedUsers { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}