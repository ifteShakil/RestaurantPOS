using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Menu;

public class MenuItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }           // ব্রাঞ্চ-অ্যাওয়্যার — override থাকলে সেটা, নাহলে BasePrice
    public string CurrencyCode { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<MenuItemVariantDto> Variants { get; set; } = new();
}

public class MenuItemVariantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal ExtraPrice { get; set; }
}
