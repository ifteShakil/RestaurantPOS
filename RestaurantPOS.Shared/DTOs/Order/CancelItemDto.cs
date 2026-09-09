using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Order;

public class CancelItemDto
{
    public int Quantity { get; set; }   // কত পিস cancel করতে চায়
}
