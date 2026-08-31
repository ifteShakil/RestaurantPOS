using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.Order;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("branch/{branchId}/active")]
    public async Task<IActionResult> GetActiveOrders(int branchId)
    {
        var orders = await _unitOfWork.Orders.GetActiveOrdersByBranchAsync(branchId);
        return Ok(_mapper.Map<List<OrderDto>>(orders));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(id);
        if (order == null) return NotFound();
        return Ok(_mapper.Map<OrderDto>(order));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        if (dto.Items.Count == 0)
            return BadRequest("Order must have at least one item.");

        var order = new Order
        {
            BranchId = dto.BranchId,
            TableId = dto.TableId,
            WaiterId = dto.WaiterId,
            Status = OrderStatus.Pending
        };

        foreach (var itemDto in dto.Items)
        {
            var menuItem = await _unitOfWork.MenuItems.GetWithVariantsAsync(itemDto.MenuItemId);
            if (menuItem == null)
                return BadRequest($"Menu item {itemDto.MenuItemId} not found.");

            // ⭐ দাম সার্ভার-সাইডে লুকআপ — ক্লায়েন্ট থেকে কখনো নেওয়া হচ্ছে না
            decimal unitPrice = menuItem.BasePrice;
            var branchOverride = menuItem.BranchPrices.FirstOrDefault(bp => bp.BranchId == dto.BranchId);
            if (branchOverride != null) unitPrice = branchOverride.Price;

            if (itemDto.MenuItemVariantId.HasValue)
            {
                var variant = menuItem.Variants.FirstOrDefault(v => v.Id == itemDto.MenuItemVariantId);
                if (variant != null) unitPrice += variant.ExtraPrice;
            }

            order.Items.Add(new OrderItem
            {
                MenuItemId = itemDto.MenuItemId,
                MenuItemVariantId = itemDto.MenuItemVariantId,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice,
                Note = itemDto.Note
            });
        }

        await _unitOfWork.Orders.AddAsync(order);

        // ⭐ নতুন — Table-কে Occupied মার্ক করা
        var table = await _unitOfWork.Tables.GetByIdAsync(dto.TableId);
        if (table != null)
        {
            table.Status = TableStatus.Occupied;
            _unitOfWork.Tables.Update(table);
        }
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Orders.GetOrderWithItemsAsync(order.Id);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, _mapper.Map<OrderDto>(created));
    }


    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null) return NotFound();

        if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
            return BadRequest("Invalid status value.");

        order.Status = newStatus;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

}
