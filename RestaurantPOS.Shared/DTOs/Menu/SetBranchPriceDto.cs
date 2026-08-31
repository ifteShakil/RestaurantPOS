using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Menu;

public class SetBranchPriceDto
{
    public int BranchId { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
}
