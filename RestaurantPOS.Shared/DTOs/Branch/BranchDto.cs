using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Branch;

public class BranchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public decimal TaxRatePercent { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public string LocaleCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
}



public class CreateBranchDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public decimal TaxRatePercent { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public string LocaleCode { get; set; } = "en-US";
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
