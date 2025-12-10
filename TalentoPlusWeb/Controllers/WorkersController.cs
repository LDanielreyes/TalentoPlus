using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Application.Services.Impl;

namespace TalentoPlusWeb.Controllers;

[Authorize(Roles = "Admin,Worker")]
public class WorkersController(IWorkerService workerService, Microsoft.AspNetCore.Identity.UserManager<TalentoPlus.Domain.Entities.Person> userManager) : Controller
{
    public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
    {
        // If user is Worker, redirect to their own details
        if (User.IsInRole("Worker"))
        {
            var userId = userManager.GetUserId(User);
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        int pageSize = 10;
        var (workers, totalCount) = await workerService.GetWorkersAsync(searchString, pageNumber, pageSize);

        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.SearchString = searchString;

        return View(workers);
    }

    public async Task<IActionResult> Details(string id)
    {
        // Workers can only view their own details
        if (User.IsInRole("Worker"))
        {
            var currentUserId = userManager.GetUserId(User);
            if (currentUserId != id)
            {
                return Forbid();
            }
        }

        var worker = await workerService.GetWorkerByIdAsync(id);
        if (worker == null)
        {
            return NotFound();
        }
        return View(worker);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Worker worker, string? Password)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Set username to email if not provided
                if (string.IsNullOrWhiteSpace(worker.UserName))
                {
                    worker.UserName = worker.Email;
                }
                
                // Set register date
                worker.RegisterDate = DateTime.UtcNow;
                worker.EmailConfirmed = true;
                
                // Use default password if not provided
                var password = string.IsNullOrWhiteSpace(Password) ? "Worker@123" : Password;
                
                // Create the worker through the service (which uses UserManager)
                await workerService.CreateWorkerAsync(worker);
                
                TempData["SuccessMessage"] = $"Worker {worker.FullName} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating worker: {ex.Message}");
            }
        }
        return View(worker);
    }

    public async Task<IActionResult> Edit(string id)
    {
        // Workers can only edit their own profile
        if (User.IsInRole("Worker"))
        {
            var currentUserId = userManager.GetUserId(User);
            if (currentUserId != id)
            {
                return Forbid();
            }
        }

        var worker = await workerService.GetWorkerByIdAsync(id);
        if (worker == null)
        {
            return NotFound();
        }
        return View(worker);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, Worker worker)
    {
        if (id != worker.Id)
        {
            return NotFound();
        }

        // Workers can only edit their own profile
        if (User.IsInRole("Worker"))
        {
            var currentUserId = userManager.GetUserId(User);
            if (currentUserId != id)
            {
                return Forbid();
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                await workerService.UpdateWorkerAsync(worker);
                TempData["SuccessMessage"] = $"Worker {worker.FullName} updated successfully!";
                
                if (User.IsInRole("Worker"))
                {
                    return RedirectToAction(nameof(Details), new { id = worker.Id });
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating worker: {ex.Message}");
            }
        }
        return View(worker);
    }

    public async Task<IActionResult> Delete(string id)
    {
        var worker = await workerService.GetWorkerByIdAsync(id);
        if (worker == null)
        {
            return NotFound();
        }
        return View(worker);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await workerService.DeleteWorkerAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    [IgnoreAntiforgeryToken] // Temporary: bypassing antiforgery validation due to DataProtection compatibility issue
    public async Task<IActionResult> Import(TalentoPlusWeb.Models.WorkerImportViewModel model)
    {
        if (ModelState.IsValid)
        {
            using var stream = model.File.OpenReadStream();
            try
            {
                await workerService.ImportWorkersFromExcelAsync(stream);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al importar el archivo: {ex.Message}");
            }
        }
        return View(model);
    }

    public async Task<IActionResult> DownloadCv(string id, [FromServices] IPdfService pdfService)
    {
        var worker = await workerService.GetWorkerByIdAsync(id);
        if (worker == null)
        {
            return NotFound();
        }

        // Workers can only download their own CV
        if (User.IsInRole("Worker"))
        {
            var currentUserId = userManager.GetUserId(User);
            if (currentUserId != id)
            {
                return Forbid();
            }
        }

        var pdfBytes = pdfService.GenerateWorkerCv(worker);
        return File(pdfBytes, "application/pdf", $"CV_{worker.FullName.Replace(" ", "_")}.pdf");
    }
}
