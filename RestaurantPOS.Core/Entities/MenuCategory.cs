using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Entities;

public class MenuCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;      // যেমন: Starter, Main Course, Dessert
    public int DisplayOrder { get; set; }
    public List<MenuItem> Items { get; set; } = new();
}

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }          // ডিফল্ট প্রাইস (কোনো ব্রাঞ্চ-ওভাররাইড না থাকলে এটা ব্যবহার হবে)
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;    // গ্লোবাল অ্যাভেইলেবিলিটি

    public int CategoryId { get; set; }
    public MenuCategory? Category { get; set; }

    public List<MenuItemVariant> Variants { get; set; } = new();
    public List<MenuItemBranchPrice> BranchPrices { get; set; } = new();
}

public class MenuItemVariant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal ExtraPrice { get; set; }

    public int MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }
}

// প্রতি ব্রাঞ্চে দাম বা স্টক আলাদা হলে এখানে override করবে
public class MenuItemBranchPrice
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }

    public int BranchId { get; set; }
    public Branch? Branch { get; set; }

    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;   // Branch.CurrencyCode-এর সাথে মিলবে
    public bool IsAvailable { get; set; } = true; // ব্রাঞ্চ-ওভাররাইড অ্যাভেইলেবিলিটি


}
