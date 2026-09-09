using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<List<Order>> GetActiveOrdersByBranchAsync(int branchId);
    Task<Order?> GetOrderWithItemsAsync(int orderId);
    Task<List<Order>> GetOrdersByTableAsync(int tableId);
    Task<DashboardSummary> GetDashboardSummaryAsync(int branchId);
    Task<int> GetTodayOrderCountAsync(int branchId);


}
