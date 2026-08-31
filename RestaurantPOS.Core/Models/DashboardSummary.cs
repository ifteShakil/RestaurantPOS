using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Models;

public class DashboardSummary
{
    public decimal TodaysSales { get; set; }
    public int ActiveOrderCount { get; set; }
    public int OccupiedTables { get; set; }
    public int TotalTables { get; set; }
}