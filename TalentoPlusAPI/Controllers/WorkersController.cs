using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlusAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkersController(IWorkerService workerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Worker>>> GetWorkers()
    {
        var workers = await workerService.GetAllWorkersAsync();
        return Ok(workers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Worker>> GetWorker(string id)
    {
        var worker = await workerService.GetWorkerByIdAsync(id);
        if (worker == null)
        {
            return NotFound();
        }
        return Ok(worker);
    }

    [HttpPost]
    public async Task<ActionResult<Worker>> CreateWorker(Worker worker)
    {
        await workerService.CreateWorkerAsync(worker);
        return CreatedAtAction(nameof(GetWorker), new { id = worker.Id }, worker);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorker(string id, Worker worker)
    {
        if (id != worker.Id)
        {
            return BadRequest();
        }
        await workerService.UpdateWorkerAsync(worker);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorker(string id)
    {
        await workerService.DeleteWorkerAsync(id);
        return NoContent();
    }
}
