using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public decimal TodaysSales { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public int ActiveOrderCount { get; set; }
    public int OccupiedTables { get; set; }
    public int TotalTables { get; set; }
}
