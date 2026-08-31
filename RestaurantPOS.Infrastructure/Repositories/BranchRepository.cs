using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Infrastructure.Repositories;

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    public BranchRepository(AppDbContext context) : base(context) { }

    public async Task<Branch?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Code == code);
    }

    public async Task<List<Branch>> GetActiveByCountryAsync(string countryCode)
    {
        return await _dbSet
            .Where(b => b.CountryCode == countryCode && b.IsActive)
            .ToListAsync();
    }
}
