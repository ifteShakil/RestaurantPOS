using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.API.Services;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.User;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TokenService _tokenService;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public AuthController(IUnitOfWork unitOfWork, TokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Username == dto.Username && u.IsActive);

        if (user == null)
            return Unauthorized("Invalid username or password.");

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid username or password.");

        var token = _tokenService.GenerateToken(user);

        string? branchName = null;
        if (user.BranchId.HasValue)
        {
            var branch = await _unitOfWork.Branches.GetByIdAsync(user.BranchId.Value);
            branchName = branch?.Name;
        }

        return Ok(new LoginResponseDto
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Role = user.Role.ToString(),
                BranchId = user.BranchId,
                BranchName = branchName,
                IsActive = user.IsActive
            }
        });
    }



}
