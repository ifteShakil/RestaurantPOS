using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.Order;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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

        var todayCount = await _unitOfWork.Orders.GetTodayOrderCountAsync(dto.BranchId);

        var order = new Order
        {
            BranchId = dto.BranchId,
            TableId = dto.TableId,
            WaiterId = dto.WaiterId,
            Status = OrderStatus.Pending,
            DailyTicketNumber = todayCount + 1
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

    [HttpPatch("{orderId}/items/{itemId}/cancel")]
    public async Task<IActionResult> CancelItem(int orderId, int itemId, CancelItemDto dto)
    {
        var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
        if (order == null) return NotFound("Order not found.");

        var item = order.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return NotFound("Item not found in this order.");

        var activeQty = item.Quantity - item.CancelledQuantity;
        if (dto.Quantity <= 0 || dto.Quantity > activeQty)
            return BadRequest($"Invalid cancel quantity. Active quantity is {activeQty}.");

        item.CancelledQuantity += dto.Quantity;
        item.LastCancelledAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return Ok(_mapper.Map<OrderItemDto>(item));
    }



    [HttpPost("{orderId}/items")]
    public async Task<IActionResult> AddItems(int orderId, AddOrderItemsDto dto)
    {
        if (dto.Items.Count == 0)
            return BadRequest("At least one item is required.");

        var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
        if (order == null) return NotFound("Order not found.");

        if (order.Status == OrderStatus.Paid || order.Status == OrderStatus.Cancelled)
            return BadRequest("Cannot add items to a paid or cancelled order.");

        if (order.Bill != null)
            return BadRequest("Bill already generated for this order. Please create a new order for additional items.");

        var newlyAddedItems = new List<OrderItem>();   // ⭐ সরাসরি reference রাখা হচ্ছে

        foreach (var itemDto in dto.Items)
        {
            var menuItem = await _unitOfWork.MenuItems.GetWithVariantsAsync(itemDto.MenuItemId);
            if (menuItem == null)
                return BadRequest($"Menu item {itemDto.MenuItemId} not found.");

            decimal unitPrice = menuItem.BasePrice;
            var branchOverride = menuItem.BranchPrices.FirstOrDefault(bp => bp.BranchId == order.BranchId);
            if (branchOverride != null) unitPrice = branchOverride.Price;

            if (itemDto.MenuItemVariantId.HasValue)
            {
                var variant = menuItem.Variants.FirstOrDefault(v => v.Id == itemDto.MenuItemVariantId);
                if (variant != null) unitPrice += variant.ExtraPrice;
            }

            var newItem = new OrderItem
            {
                OrderId = order.Id,
                MenuItemId = itemDto.MenuItemId,
                MenuItemVariantId = itemDto.MenuItemVariantId,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice,
                Note = itemDto.Note,
                CreatedAt = DateTime.UtcNow
            };

            order.Items.Add(newItem);
            newlyAddedItems.Add(newItem);   // ⭐ reference সংরক্ষণ
        }

        if (order.Status == OrderStatus.Served || order.Status == OrderStatus.Ready)
            order.Status = OrderStatus.InKitchen;

        await _unitOfWork.SaveChangesAsync();   // ⭐ SaveChanges-এর পর newItem.Id স্বয়ংক্রিয়ভাবে পপুলেট হয়ে যায়

        var updated = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
        var orderDto = _mapper.Map<OrderDto>(updated);

        return Ok(new AddOrderItemsResponseDto
        {
            Order = orderDto,
            NewItemIds = newlyAddedItems.Select(i => i.Id).ToList()   // ⭐ নির্ভরযোগ্য
        });
    }

    //[HttpPost("{orderId}/items")]
    //public async Task<IActionResult> AddItems(int orderId, AddOrderItemsDto dto)
    //{
    //    if (dto.Items.Count == 0)
    //        return BadRequest("At least one item is required.");

    //    var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
    //    if (order == null) return NotFound("Order not found.");

    //    if (order.Status == OrderStatus.Paid || order.Status == OrderStatus.Cancelled)
    //        return BadRequest("Cannot add items to a paid or cancelled order.");

    //    if (order.Bill != null)
    //        return BadRequest("Bill already generated for this order. Please create a new order for additional items.");

    //    var newItemIds = new List<int>();

    //    foreach (var itemDto in dto.Items)
    //    {
    //        var menuItem = await _unitOfWork.MenuItems.GetWithVariantsAsync(itemDto.MenuItemId);
    //        if (menuItem == null)
    //            return BadRequest($"Menu item {itemDto.MenuItemId} not found.");

    //        decimal unitPrice = menuItem.BasePrice;
    //        var branchOverride = menuItem.BranchPrices.FirstOrDefault(bp => bp.BranchId == order.BranchId);
    //        if (branchOverride != null) unitPrice = branchOverride.Price;

    //        if (itemDto.MenuItemVariantId.HasValue)
    //        {
    //            var variant = menuItem.Variants.FirstOrDefault(v => v.Id == itemDto.MenuItemVariantId);
    //            if (variant != null) unitPrice += variant.ExtraPrice;
    //        }

    //        var newItem = new OrderItem
    //        {
    //            OrderId = order.Id,
    //            MenuItemId = itemDto.MenuItemId,
    //            MenuItemVariantId = itemDto.MenuItemVariantId,
    //            Quantity = itemDto.Quantity,
    //            UnitPrice = unitPrice,
    //            Note = itemDto.Note,
    //            CreatedAt = DateTime.UtcNow
    //        };

    //        order.Items.Add(newItem);
    //    }

    //    // যদি অর্ডার আগে থেকে Served/Ready থাকে, নতুন আইটেম আসায় আবার কিচেনে যেতে হবে
    //    if (order.Status == OrderStatus.Served || order.Status == OrderStatus.Ready)
    //        order.Status = OrderStatus.InKitchen;

    //    await _unitOfWork.SaveChangesAsync();

    //    // ⭐ এইমাত্র যোগ হওয়া লাইনগুলোর Id সংগ্রহ (SaveChanges-এর পরই DB-generated Id পাওয়া যায়)
    //    var addedItems = order.Items.Where(i => dto.Items.Any(d =>
    //        d.MenuItemId == i.MenuItemId && i.CreatedAt > DateTime.UtcNow.AddSeconds(-5))).ToList();

    //    var updated = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
    //    var orderDto = _mapper.Map<OrderDto>(updated);

    //    return Ok(new AddOrderItemsResponseDto
    //    {
    //        Order = orderDto,
    //        NewItemIds = addedItems.Select(i => i.Id).ToList()
    //    });
    //}




}
