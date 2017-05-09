using InventoryManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace InventoryManager
{
    public class InventoryListViewModel
    {
        public List<InventoryViewModel> Owned { get; set; }

        public List<InventoryViewModel> Shared { get; set; }
    }

    public class InventoryViewModel
    {
        public static Expression<Func<Inventory, InventoryViewModel>> GetInstantiator(string userId, string email)
        {
            return e => new InventoryViewModel
            {
                DefaultCategory = e.DefaultCategory,
                DefaultColor = e.DefaultColor,
                DefaultInStock = e.DefaultInStock,
                DefaultName = e.DefaultName,
                DefaultSize = e.DefaultSize,
                DefaultTotal = e.DefaultTotal,
                DefaultType = e.DefaultType,
                Id = e.Id,
                Items = e.Items,
                Name = e.Name,
                SharedUsers = e.SharedUsers,
                UserId = e.UserId,
                IsOwner = e.UserId == userId,
                ShareRecord = e.SharedUsers.Where(s => s.Email == email).FirstOrDefault()
            };
        }

        public bool IsOwner { get; set; }

        public Sharing ShareRecord { get; set; }

        public AccessLevel? Permission
        {
            get
            {
                return IsOwner ? AccessLevel.Admin : ShareRecord?.Permission;
            }
        }
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
        public ICollection<Item> Items { get; set; }

        public ICollection<Sharing> SharedUsers { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}