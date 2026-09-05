using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Shared.DTOs.Dashboard;

namespace RestaurantPOS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    public DashboardController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    [HttpGet("branch/{branchId}/summary")]
    public async Task<IActionResult> GetSummary(int branchId)
    {
        var branch = await _unitOfWork.Branches.GetByIdAsync(branchId);
        if (branch == null) return NotFound("Branch not found.");

        var summary = await _unitOfWork.Orders.GetDashboardSummaryAsync(branchId);

        return Ok(new DashboardSummaryDto
        {
            TodaysSales = summary.TodaysSales,
            CurrencyCode = branch.CurrencyCode,
            ActiveOrderCount = summary.ActiveOrderCount,
            OccupiedTables = summary.OccupiedTables,
            TotalTables = summary.TotalTables
        });
    }
}
