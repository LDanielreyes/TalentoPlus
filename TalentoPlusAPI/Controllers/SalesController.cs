using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlusAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController(ISaleService saleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
    {
        var sales = await saleService.GetAllSalesAsync();
        return Ok(sales);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetSale(int id)
    {
        var sale = await saleService.GetSaleByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }
        return Ok(sale);
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> CreateSale(Sale sale)
    {
        await saleService.CreateSaleAsync(sale);
        return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSale(int id, Sale sale)
    {
        if (id != sale.Id)
        {
            return BadRequest();
        }
        await saleService.UpdateSaleAsync(sale);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSale(int id)
    {
        await saleService.DeleteSaleAsync(id);
        return NoContent();
    }

    [HttpGet("worker/{workerId}")]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSalesByWorker(string workerId)
    {
        var sales = await saleService.GetSalesByWorkerIdAsync(workerId);
        return Ok(sales);
    }
}
