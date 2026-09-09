using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<List<Order>> GetActiveOrdersByBranchAsync(int branchId)
    {
        return await _dbSet
            .Where(o => o.BranchId == branchId && o.Status != OrderStatus.Paid && o.Status != OrderStatus.Cancelled)
            .Include(o => o.Table)
            .Include(o => o.Waiter)
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.Items).ThenInclude(i => i.MenuItem)
            .Include(o => o.Items).ThenInclude(i => i.MenuItemVariant)
            .Include(o => o.Table)
            .Include(o => o.Bill)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<List<Order>> GetOrdersByTableAsync(int tableId)
    {
        return await _dbSet
            .Where(o => o.TableId == tableId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }


    public async Task<DashboardSummary> GetDashboardSummaryAsync(int branchId)
    {
        var today = DateTime.UtcNow.Date;

        var todaysSales = await _context.Bills
            .Where(b => b.BranchId == branchId
                     && b.GeneratedAt.Date == today
                     && b.Order!.Status == OrderStatus.Paid)
            .SumAsync(b => (decimal?)b.TotalAmount) ?? 0;

        var activeOrderCount = await _context.Orders
            .CountAsync(o => o.BranchId == branchId && o.Status != OrderStatus.Paid && o.Status != OrderStatus.Cancelled);

        var occupiedTables = await _context.RestaurantTables
            .CountAsync(t => t.BranchId == branchId && t.Status == TableStatus.Occupied);

        var totalTables = await _context.RestaurantTables
            .CountAsync(t => t.BranchId == branchId);

        return new DashboardSummary
        {
            TodaysSales = todaysSales,
            ActiveOrderCount = activeOrderCount,
            OccupiedTables = occupiedTables,
            TotalTables = totalTables
        };
    }

    public async Task<int> GetTodayOrderCountAsync(int branchId)
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet.CountAsync(o => o.BranchId == branchId && o.CreatedAt.Date == today);
    }

}
