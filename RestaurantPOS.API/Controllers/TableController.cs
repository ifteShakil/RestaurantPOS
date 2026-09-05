using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.Table;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TableController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TableController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var tables = await _unitOfWork.Tables.GetAllAsync();
        var filtered = tables.Where(t => t.BranchId == branchId).ToList();
        return Ok(_mapper.Map<List<TableDto>>(filtered));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(id);
        if (table == null) return NotFound($"Table with Id {id} not found.");
        return Ok(_mapper.Map<TableDto>(table));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTableDto dto)
    {
        var branch = await _unitOfWork.Branches.GetByIdAsync(dto.BranchId);
        if (branch == null) return NotFound($"Branch {dto.BranchId} not found.");

        var table = _mapper.Map<RestaurantTable>(dto);
        await _unitOfWork.Tables.AddAsync(table);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = table.Id }, _mapper.Map<TableDto>(table));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateTableStatusDto dto)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(id);
        if (table == null) return NotFound();

        if (!Enum.TryParse<TableStatus>(dto.Status, true, out var newStatus))
            return BadRequest("Invalid status. Valid values: Available, Occupied, Reserved, NeedsCleaning");

        table.Status = newStatus;
        _unitOfWork.Tables.Update(table);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(id);
        if (table == null) return NotFound();

        _unitOfWork.Tables.Delete(table);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }



}
