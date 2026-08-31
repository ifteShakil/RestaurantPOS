using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!await context.Branches.AnyAsync())
        {
            context.Branches.Add(new Branch
            {
                Name = "Mbabane Main Branch",
                Code = "MBB-01",
                CountryCode = "SZ",
                CurrencyCode = "SZL",
                CurrencySymbol = "E",
                TaxRatePercent = 15.00m,
                TimeZoneId = "Africa/Mbabane",
                LocaleCode = "en-SZ",
                Address = "Mbabane, Eswatini"
            });

            await context.SaveChangesAsync();
        }
    }
}
