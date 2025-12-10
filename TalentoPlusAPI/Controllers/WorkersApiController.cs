using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlusAPI.Controllers;

[Route("api/workers")]
[ApiController]
public class WorkersApiController(
    UserManager<Person> userManager,
    IWorkerService workerService,
    IEmailService emailService,
    IPdfService pdfService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterWorkerRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest("Email already registered");
        }

        var worker = new Worker
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            DocumentoIdentidad = request.DocumentoIdentidad,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            Department = request.Department,
            RegisterDate = DateTime.UtcNow,
            EmailConfirmed = true, // Auto-confirm for now
            Status = WorkerStatus.Activo,
            EducationalLevel = EducationalLevel.Tecnico, // Default
            Wage = 0 // Default
        };

        try
        {
            // Create worker using WorkerService logic if possible, but here we need custom password handling
            // Using UserManager directly for simplicity in API
            var result = await userManager.CreateAsync(worker, request.Password);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(worker, "Worker");

                // Send Welcome Email
                var subject = "Bienvenido a TalentoPlus";
                var body = $"<h1>Hola {worker.FullName},</h1><p>Tu registro en TalentoPlus ha sido exitoso.</p><p>Ya puedes iniciar sesión en nuestra plataforma.</p>";
                
                // Fire and forget email or await it? User asked for real email.
                await emailService.SendEmailAsync(worker.Email, subject, body);

                return Ok(new { Message = "Registration successful. Welcome email sent." });
            }
            
            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyInfo()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var worker = await workerService.GetWorkerByIdAsync(userId);
        if (worker == null) return NotFound("Worker profile not found");

        return Ok(worker);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("me/cv")]
    public async Task<IActionResult> DownloadMyCv()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var worker = await workerService.GetWorkerByIdAsync(userId);
        if (worker == null) return NotFound("Worker profile not found");

        var pdfBytes = pdfService.GenerateWorkerCv(worker);
        return File(pdfBytes, "application/pdf", $"CV_{worker.FullName.Replace(" ", "_")}.pdf");
    }
}

public class RegisterWorkerRequest
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string DocumentoIdentidad { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public Department Department { get; set; }
}
