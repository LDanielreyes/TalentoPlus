using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;

namespace TalentoPlusWeb.Controllers;

[Authorize(Roles = "Admin")]
public class AdminsController(IAdminService adminService) : Controller
{
    public async Task<IActionResult> Dashboard()
    {
        var totalWorkers = await adminService.GetTotalWorkersAsync();
        var totalSales = await adminService.GetTotalSalesAmountAsync();
        var workersOnVacation = await adminService.GetWorkersOnVacationAsync();
        var activeWorkers = await adminService.GetActiveWorkersAsync();

        ViewBag.TotalWorkers = totalWorkers;
        ViewBag.TotalSales = totalSales;
        ViewBag.WorkersOnVacation = workersOnVacation;
        ViewBag.ActiveWorkers = activeWorkers;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProcessAIQuery([FromBody] AIQueryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Query))
        {
            return BadRequest("Query cannot be empty");
        }

        var response = await adminService.ProcessAIQueryAsync(request.Query);
        return Ok(new { Response = response });
    }
}

public class AIQueryRequest
{
    public string Query { get; set; } = string.Empty;
}
