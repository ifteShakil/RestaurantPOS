using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Order;

public class CreateOrderDto
{
    public int BranchId { get; set; }
    public int TableId { get; set; }
    public int WaiterId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public int MenuItemId { get; set; }
    public int? MenuItemVariantId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}

//কেন UnitPrice CreateOrderItemDto-তে নেই?
//এটা ইচ্ছাকৃত — দাম কখনোই ক্লায়েন্ট থেকে নেওয়া উচিত না (কেউ চাইলে রিকোয়েস্ট মডিফাই করে দাম কমিয়ে পাঠাতে পারবে)।
//সার্ভার-সাইডে MenuItem/MenuItemBranchPrice থেকে দাম লুকআপ করে বসাতে হবে।