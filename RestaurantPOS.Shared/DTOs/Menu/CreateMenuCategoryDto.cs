using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Menu;

public class CreateMenuCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}