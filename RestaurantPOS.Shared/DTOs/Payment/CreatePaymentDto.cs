using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Payment;

public class CreatePaymentDto
{
    public int BillId { get; set; }
    public int PaymentMethodTypeId { get; set; }
    public decimal Amount { get; set; }
    public string? TransactionRef { get; set; }
}


