using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IBranchRepository Branches { get; }
    public IOrderRepository Orders { get; }
    public IMenuItemRepository MenuItems { get; }
    public IGenericRepository<MenuCategory> MenuCategories { get; }
    public IGenericRepository<Bill> Bills { get; }
    public IGenericRepository<Payment> Payments { get; }
    public IGenericRepository<AppUser> Users { get; }
    public IGenericRepository<RestaurantTable> Tables { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Branches = new BranchRepository(context);
        Orders = new OrderRepository(context);
        MenuItems = new MenuItemRepository(context);
        MenuCategories = new GenericRepository<MenuCategory>(context);
        Bills = new GenericRepository<Bill>(context);
        Payments = new GenericRepository<Payment>(context);
        Users = new GenericRepository<AppUser>(context);
        Tables = new GenericRepository<RestaurantTable>(context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();

}
