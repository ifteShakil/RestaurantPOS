using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Order;

public class AddOrderItemsDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class AddOrderItemsResponseDto
{
    public OrderDto Order { get; set; } = new();
    public List<int> NewItemIds { get; set; } = new();   // ⭐ কোন লাইনগুলো এইমাত্র যোগ হলো, KOT-এ ফিল্টার করতে লাগবে
}
