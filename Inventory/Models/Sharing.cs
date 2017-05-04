using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Inventory.Models
{
    public enum AccessLevel
    {
        Read,
        Add,
        Edit,
        Admin
    }

    public class Sharing
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public AccessLevel Permission { get; set; }
    }
}