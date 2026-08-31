using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Payment;

public class BillDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxRatePercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public List<PaymentDto> Payments { get; set; } = new();
}

public class PaymentDto
{
    public int Id { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string? TransactionRef { get; set; }
    public DateTime PaidAt { get; set; }
}
