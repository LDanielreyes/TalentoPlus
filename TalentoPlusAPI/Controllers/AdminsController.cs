using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlusAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminsController(IAdminService adminService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<Admin>> GetAdmin(string id)
    {
        var admin = await adminService.GetAdminByIdAsync(id);
        if (admin == null)
        {
            return NotFound();
        }
        return Ok(admin);
    }

    [HttpGet("stats/workers")]
    public async Task<ActionResult<int>> GetTotalWorkers()
    {
        var count = await adminService.GetTotalWorkersAsync();
        return Ok(count);
    }

    [HttpGet("stats/sales")]
    public async Task<ActionResult<decimal>> GetTotalSalesAmount()
    {
        var amount = await adminService.GetTotalSalesAmountAsync();
        return Ok(amount);
    }
}
