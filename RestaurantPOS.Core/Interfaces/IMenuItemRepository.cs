using RestaurantPOS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Interfaces
{
    public interface IMenuItemRepository : IGenericRepository<MenuItem>
    {
        Task<List<MenuItem>> GetMenuByBranchAsync(int branchId);
        Task<MenuItem?> GetWithVariantsAsync(int menuItemId);
    }
}
