using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Entities;

public enum TableStatus
{
    Available,
    Occupied,
    Reserved,
    NeedsCleaning
}

public class RestaurantTable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public TableStatus Status { get; set; } = TableStatus.Available;

    public int BranchId { get; set; }
    public Branch? Branch { get; set; }

    public List<Order> Orders { get; set; } = new();
}
