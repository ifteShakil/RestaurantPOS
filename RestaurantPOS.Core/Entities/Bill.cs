using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Core.Entities;

public enum PaymentMethod
{
    Cash,
    Card,
    Bkash,
    Nagad,
    Other
}

public class Bill
{
    public int Id { get; set; }

    public int BranchId { get; set; }
    public Branch? Branch { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public decimal SubTotal { get; set; }
    public decimal TaxRatePercent { get; set; }      // বিল জেনারেট হওয়ার সময়ের রেট (স্ন্যাপশট — পরে রেট বদলালেও পুরনো বিল ঠিক থাকবে)
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;   // বিলের কারেন্সি (Branch থেকে আসবে)

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public List<Payment> Payments { get; set; } = new();


}

public class Payment
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public Bill? Bill { get; set; }

    public int PaymentMethodTypeId { get; set; }             // ⭐ enum-এর বদলে FK
    public PaymentMethodType? PaymentMethodType { get; set; }

    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;  // যে কারেন্সিতে পেমেন্ট হলো
    public string? TransactionRef { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

}


public class PaymentMethodType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;       // "Cash", "Card", "MTN MoMo", "bKash"
    public string Code { get; set; } = string.Empty;        // "CASH", "CARD", "MTN_MOMO", "BKASH"
    public bool RequiresTransactionRef { get; set; }        // mobile money/card হলে true
    public bool IsActive { get; set; } = true;

    // কোনো নির্দিষ্ট country-তেই শুধু দেখাবে (null হলে সব দেশে দেখাবে, যেমন Cash/Card)
    public string? CountryCode { get; set; }
}