using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Entities;

public enum UserRole
{
    SuperAdmin,    // হেড অফিস, সব ব্রাঞ্চ দেখতে পাবে
    BranchManager,
    Cashier,
    Waiter,
    KitchenStaff
}

public class AppUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // SuperAdmin-এর কোনো নির্দিষ্ট ব্রাঞ্চ নেই, তাই nullable
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }
}
