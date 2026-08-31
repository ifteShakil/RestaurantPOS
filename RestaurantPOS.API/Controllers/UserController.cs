using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.User;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public UserController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var filtered = users.Where(u => u.BranchId == branchId).ToList();
        return Ok(_mapper.Map<List<UserDto>>(filtered));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return NotFound($"User with Id {id} not found.");
        return Ok(_mapper.Map<UserDto>(user));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
            return BadRequest("Invalid role. Valid values: SuperAdmin, BranchManager, Cashier, Waiter, KitchenStaff");

        // Username ইউনিক কিনা চেক করা
        var existingUsers = await _unitOfWork.Users.GetAllAsync();
        if (existingUsers.Any(u => u.Username == dto.Username))
            return Conflict($"Username '{dto.Username}' already exists.");

        // SuperAdmin ছাড়া বাকি সব রোলের জন্য BranchId বাধ্যতামূলক
        if (role != UserRole.SuperAdmin && dto.BranchId == null)
            return BadRequest("BranchId is required for this role.");

        var user = new AppUser
        {
            FullName = dto.FullName,
            Username = dto.Username,
            Role = role,
            BranchId = dto.BranchId
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);   // ⭐ প্লেইন টেক্সট কখনো সেভ হচ্ছে না

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, _mapper.Map<UserDto>(user));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null) return NotFound();

        user.IsActive = false;   // soft delete — Order history-তে WaiterId রেফারেন্স থাকে
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }



}
