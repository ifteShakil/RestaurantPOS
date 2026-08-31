using RestaurantPOS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBranchRepository Branches { get; }
    IOrderRepository Orders { get; }
    IMenuItemRepository MenuItems { get; }
    IGenericRepository<MenuCategory> MenuCategories { get; }
    IGenericRepository<Bill> Bills { get; }
    IGenericRepository<Payment> Payments { get; }
    IGenericRepository<AppUser> Users { get; }
    IGenericRepository<RestaurantTable> Tables { get; }

    Task<int> SaveChangesAsync();
}
