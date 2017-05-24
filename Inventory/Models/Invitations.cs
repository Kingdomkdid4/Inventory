using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryManager.Models
{
    public class Invitations
    {

        [Required, Display(Name = "Your name")]
        public string FromName = "InventoryIt.com"; 
        [Required, Display(Name = "Your email"), EmailAddress]
        public string FromEmail = "inventory.it.invitation@gmail.com";
        [Required]
        public string Message = "Someone has shared an Inventory with you at InventoryIt.com. To use this inventory click the link http://localhost:53683/Account/Register and create an account.";

    }
}