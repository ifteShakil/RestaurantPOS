using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Table;

public class UpdateTableStatusDto
{
    public string Status { get; set; } = string.Empty;   // "Available", "Occupied", "Reserved", "NeedsCleaning"
}
