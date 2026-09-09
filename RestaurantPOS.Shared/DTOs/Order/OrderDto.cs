using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public int DailyTicketNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public int TableId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public int WaiterId { get; set; }
    public string WaiterName { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }             // ⭐ TotalAmount property to hold the total amount of the order
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public int? MenuItemVariantId { get; set; }
    public string? VariantName { get; set; }
    public int Quantity { get; set; }
    public int CancelledQuantity { get; set; }            
    public int ActiveQuantity => Quantity - CancelledQuantity;   // Computed property for active quantity

    public decimal UnitPrice { get; set; }
    public string? Note { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; }
}
