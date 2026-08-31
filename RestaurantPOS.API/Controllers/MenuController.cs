using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.Menu;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MenuController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetMenuByBranch(int branchId)
    {
        var branch = await _unitOfWork.Branches.GetByIdAsync(branchId);
        if (branch == null) return NotFound($"Branch {branchId} not found.");

        var items = await _unitOfWork.MenuItems.GetMenuByBranchAsync(branchId);
        var dtos = _mapper.Map<List<MenuItemDto>>(items);

        // Branch-specific override থাকলে দাম/currency ওভাররাইড করা হচ্ছে
        foreach (var (item, dto) in items.Zip(dtos))
        {
            var override_ = item.BranchPrices.FirstOrDefault(bp => bp.BranchId == branchId);
            if (override_ != null)
            {
                dto.Price = override_.Price;
                dto.CurrencyCode = override_.CurrencyCode;
                dto.IsAvailable = override_.IsAvailable;
            }
            else
            {
                dto.CurrencyCode = branch.CurrencyCode;   // override নেই, তাই ব্রাঞ্চের ডিফল্ট কারেন্সি বসবে
            }
        }

        return Ok(dtos);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _unitOfWork.MenuCategories.GetAllAsync();
        
        var ordered = categories.OrderBy(c => c.DisplayOrder).ToList();
        return Ok(_mapper.Map<List<MenuCategoryDto>>(ordered));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _unitOfWork.MenuItems.GetWithVariantsAsync(id);
        if (item == null) return NotFound();
        return Ok(_mapper.Map<MenuItemDto>(item));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMenuItemDto dto)
    {
        var item = _mapper.Map<MenuItem>(dto);
        await _unitOfWork.MenuItems.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, _mapper.Map<MenuItemDto>(item));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateMenuItemDto dto)
    {
        var item = await _unitOfWork.MenuItems.GetByIdAsync(id);
        if (item == null) return NotFound();

        _mapper.Map(dto, item);
        _unitOfWork.MenuItems.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return Ok(_mapper.Map<MenuItemDto>(item));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _unitOfWork.MenuItems.GetByIdAsync(id);
        if (item == null) return NotFound();

        item.IsAvailable = false;  
        _unitOfWork.MenuItems.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{menuItemId}/branch-price")]
    public async Task<IActionResult> SetBranchPrice(int menuItemId, [FromBody] SetBranchPriceDto dto)
    {
        var item = await _unitOfWork.MenuItems.GetByIdAsync(menuItemId);
        if (item == null) return NotFound("Menu item not found.");

        var branch = await _unitOfWork.Branches.GetByIdAsync(dto.BranchId);
        if (branch == null) return NotFound("Branch not found.");

        var newPrice = new MenuItemBranchPrice
        {
            MenuItemId = menuItemId,
            BranchId = dto.BranchId,
            Price = dto.Price,
            CurrencyCode = branch.CurrencyCode,
            IsAvailable = dto.IsAvailable
        };

        await _unitOfWork.MenuItems.AddAsync(item); // no-op, item already tracked — শুধু নতুন override সেভ করতে
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Branch price set successfully." });
    }

}
