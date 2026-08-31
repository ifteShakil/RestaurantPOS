using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.Menu;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MenuCategoryController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MenuCategoryController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _unitOfWork.MenuCategories.GetAllAsync();
        var allMenuItems = await _unitOfWork.MenuItems.GetAllAsync();

        var ordered = categories.OrderBy(c => c.DisplayOrder).ToList();
        var dtos = _mapper.Map<List<MenuCategoryDto>>(ordered);

        // ⭐ প্রতিটা category-তে সঠিক item count বসানো
        foreach (var dto in dtos)
        {
            dto.Items = allMenuItems.Where(m => m.CategoryId == dto.Id)
                                      .Select(m => _mapper.Map<MenuItemDto>(m))
                                      .ToList();
        }

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _unitOfWork.MenuCategories.GetByIdAsync(id);
        if (category == null) return NotFound($"Category with Id {id} not found.");
        return Ok(_mapper.Map<MenuCategoryDto>(category));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateMenuCategoryDto dto)
    {
        var category = _mapper.Map<MenuCategory>(dto);
        await _unitOfWork.MenuCategories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, _mapper.Map<MenuCategoryDto>(category));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateMenuCategoryDto dto)
    {
        var category = await _unitOfWork.MenuCategories.GetByIdAsync(id);
        if (category == null) return NotFound();

        _mapper.Map(dto, category);
        _unitOfWork.MenuCategories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return Ok(_mapper.Map<MenuCategoryDto>(category));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _unitOfWork.MenuCategories.GetByIdAsync(id);
        if (category == null) return NotFound();

        _unitOfWork.MenuCategories.Delete(category);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }


}
