using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Entities;

public enum OrderStatus
{
    Pending,      // অর্ডার নেওয়া হয়েছে
    InKitchen,    // কিচেনে পাঠানো হয়েছে
    Ready,        // রান্না শেষ
    Served,       // টেবিলে দেওয়া হয়েছে
    Paid,         // বিল পরিশোধ
    Cancelled
}

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public int BranchId { get; set; }               // ⭐ প্রতিটি অর্ডার নির্দিষ্ট ব্রাঞ্চের
    public Branch? Branch { get; set; }

    public int TableId { get; set; }
    public RestaurantTable? Table { get; set; }

    public int WaiterId { get; set; }
    public AppUser? Waiter { get; set; }

    public List<OrderItem> Items { get; set; } = new();
    public Bill? Bill { get; set; }

    public int DailyTicketNumber { get; set; } // প্রতিদিনের অর্ডারের জন্য ক্রমিক সংখ্যা, যা প্রতিটি ব্রাঞ্চের জন্য আলাদা হবে
}

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }     // অর্ডারের সময়ের প্রাইস (দাম পরে বদলালেও বিল ঠিক থাকবে)
    public string? Note { get; set; }          // যেমন: ঝাল কম

    public int CancelledQuantity { get; set; } = 0;      // ⭐ IsCancelled bool-এর বদলে
    public DateTime? LastCancelledAt { get; set; }        // ⭐ CancelledAt-এর জায়গায় রিনেম
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public int MenuItemId { get; set; }
    public MenuItem? MenuItem { get; set; }

    public int? MenuItemVariantId { get; set; }
    public MenuItemVariant? MenuItemVariant { get; set; }

    public bool IsCancelled { get; set; } = false;   
    public DateTime? CancelledAt { get; set; }        
}