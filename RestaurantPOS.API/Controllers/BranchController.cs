using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.Branch;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BranchController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BranchController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var branches = await _unitOfWork.Branches.GetAllAsync();
        return Ok(_mapper.Map<List<BranchDto>>(branches));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var branch = await _unitOfWork.Branches.GetByIdAsync(id);
        if (branch == null) return NotFound($"Branch with Id {id} not found.");
        return Ok(_mapper.Map<BranchDto>(branch));
    }

    [HttpGet("country/{countryCode}")]
    public async Task<IActionResult> GetByCountry(string countryCode)
    {
        var branches = await _unitOfWork.Branches.GetActiveByCountryAsync(countryCode);
        return Ok(_mapper.Map<List<BranchDto>>(branches));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create(CreateBranchDto dto)
    {
        var existing = await _unitOfWork.Branches.GetByCodeAsync(dto.Code);
        if (existing != null)
            return Conflict($"Branch code '{dto.Code}' already exists.");

        var branch = _mapper.Map<Branch>(dto);
        await _unitOfWork.Branches.AddAsync(branch);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = branch.Id }, _mapper.Map<BranchDto>(branch));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Update(int id, CreateBranchDto dto)
    {
        var branch = await _unitOfWork.Branches.GetByIdAsync(id);
        if (branch == null) return NotFound();

        _mapper.Map(dto, branch);   // existing entity-তে ওভাররাইট করবে
        _unitOfWork.Branches.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return Ok(_mapper.Map<BranchDto>(branch));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var branch = await _unitOfWork.Branches.GetByIdAsync(id);
        if (branch == null) return NotFound();

        branch.IsActive = false;   // soft delete — hard delete করলে Order history-র সাথে conflict হবে
        _unitOfWork.Branches.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }


}
