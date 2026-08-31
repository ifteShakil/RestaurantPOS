using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.User;

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;   // plain text — কন্ট্রোলারে হ্যাশ হবে
    public string Role { get; set; } = string.Empty;        // "SuperAdmin", "BranchManager", "Cashier", "Waiter", "KitchenStaff"
    public int? BranchId { get; set; }
}
