using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.Payment;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BillController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost("generate/{orderId}")]
    public async Task<IActionResult> GenerateBill(int orderId)
    {
        var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
        if (order == null) return NotFound("Order not found.");
        if (order.Bill != null) return Conflict("Bill already generated for this order.");

        var branch = await _unitOfWork.Branches.GetByIdAsync(order.BranchId);
        if (branch == null) return NotFound("Branch not found.");

        decimal subTotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        decimal taxAmount = subTotal * (branch.TaxRatePercent / 100);

        var bill = new Bill
        {
            OrderId = order.Id,
            BranchId = order.BranchId,
            SubTotal = subTotal,
            TaxRatePercent = branch.TaxRatePercent,   // স্ন্যাপশট — পরে ব্রাঞ্চের ট্যাক্স রেট বদলালেও এই বিল ঠিক থাকবে
            TaxAmount = taxAmount,
            DiscountAmount = 0,
            TotalAmount = subTotal + taxAmount,
            CurrencyCode = branch.CurrencyCode
        };

        await _unitOfWork.Bills.AddAsync(bill);
        order.Status = OrderStatus.Served;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return Ok(_mapper.Map<BillDto>(bill));
    }

    [HttpPost("payment")]
    public async Task<IActionResult> AddPayment(CreatePaymentDto dto)
    {
        var bill = await _unitOfWork.Bills.GetByIdAsync(dto.BillId);
        if (bill == null) return NotFound("Bill not found.");

        var payment = new Payment
        {
            BillId = dto.BillId,
            PaymentMethodTypeId = dto.PaymentMethodTypeId,
            Amount = dto.Amount,
            CurrencyCode = bill.CurrencyCode,
            TransactionRef = dto.TransactionRef
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        // পুরো বিল পরিশোধ হয়ে গেলে সংশ্লিষ্ট Order-কে Paid মার্ক করা
        var order = await _unitOfWork.Orders.GetByIdAsync(bill.OrderId);
        if (order != null)
        {
            order.Status = OrderStatus.Paid;
            _unitOfWork.Orders.Update(order);

            var table = await _unitOfWork.Tables.GetByIdAsync(order.TableId);
            if (table != null)
            {
                table.Status = TableStatus.Available;
                _unitOfWork.Tables.Update(table);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        return Ok(_mapper.Map<PaymentDto>(payment));
    }


}
