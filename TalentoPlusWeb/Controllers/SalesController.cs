using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlusWeb.Controllers;

[Authorize(Roles = "Admin")]
public class SalesController(ISaleService saleService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var sales = await saleService.GetAllSalesAsync();
        return View(sales);
    }

    public async Task<IActionResult> Details(int id)
    {
        var sale = await saleService.GetSaleByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }
        return View(sale);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Sale sale)
    {
        if (ModelState.IsValid)
        {
            await saleService.CreateSaleAsync(sale);
            return RedirectToAction(nameof(Index));
        }
        return View(sale);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var sale = await saleService.GetSaleByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }
        return View(sale);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Sale sale)
    {
        if (id != sale.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await saleService.UpdateSaleAsync(sale);
            return RedirectToAction(nameof(Index));
        }
        return View(sale);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var sale = await saleService.GetSaleByIdAsync(id);
        if (sale == null)
        {
            return NotFound();
        }
        return View(sale);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await saleService.DeleteSaleAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
