using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public int WaiterId { get; set; }
    public string WaiterName { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }             // দ্রুত preview-এর জন্য, বিস্তারিত Bill আলাদা এন্ডপয়েন্টে
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int? MenuItemVariantId { get; set; }
    public string? VariantName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Note { get; set; }
}
