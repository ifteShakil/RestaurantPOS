using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Entities;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;       // যেমন: Dhanmondi Branch
    public string Code { get; set; } = string.Empty;       // যেমন: DHM-01 (ইউনিক শর্ট কোড)

    public string CountryCode { get; set; } = string.Empty;   // ISO 3166-1 alpha-2, যেমন: "SZ" (Eswatini), "BD"
    public string CurrencyCode { get; set; } = string.Empty;  // ISO 4217, যেমন: "SZL", "BDT"
    public string CurrencySymbol { get; set; } = string.Empty; // "E" (Lilangeni), "৳"
    public decimal TaxRatePercent { get; set; }                // যেমন: 15.00 (Eswatini VAT)
    public string TimeZoneId { get; set; } = string.Empty;     // যেমন: "Africa/Mbabane", "Asia/Dhaka"
    public string LocaleCode { get; set; } = "en-US";           // en-SZ, bn-BD ইত্যাদি — ফরম্যাটিং-এর জন্য

    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<RestaurantTable> Tables { get; set; } = new();
    public List<AppUser> Users { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<MenuItemBranchPrice> MenuItemPrices { get; set; } = new();
}
