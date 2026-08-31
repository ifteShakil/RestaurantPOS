using RestaurantPOS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Interfaces;

public interface IBranchRepository : IGenericRepository<Branch>
{
    Task<Branch?> GetByCodeAsync(string code);
    Task<List<Branch>> GetActiveByCountryAsync(string countryCode);
}
