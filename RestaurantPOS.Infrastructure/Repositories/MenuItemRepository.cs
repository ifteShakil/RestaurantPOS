using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Infrastructure.Repositories;

public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(AppDbContext context) : base(context) { }

    public async Task<List<MenuItem>> GetMenuByBranchAsync(int branchId)
    {
        // BasePrice ব্যবহার হবে, কিন্তু কোনো ব্রাঞ্চ-স্পেসিফিক override থাকলে সেটা প্রাধান্য পাবে
        return await _dbSet
            .Include(m => m.Category)
            .Include(m => m.Variants)
            .Include(m => m.BranchPrices.Where(bp => bp.BranchId == branchId))
            .Where(m => m.IsAvailable)
            .ToListAsync();
    }

    public async Task<MenuItem?> GetWithVariantsAsync(int menuItemId)
    {
        return await _dbSet
            .Include(m => m.Variants)
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Id == menuItemId);
    }
}
