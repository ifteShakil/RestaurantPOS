using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Table;

public class CreateTableDto
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int BranchId { get; set; }
}
