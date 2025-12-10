using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Domain.Enums;

namespace TalentoPlusAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetDepartments()
    {
        var departments = Enum.GetValues(typeof(Department))
                              .Cast<Department>()
                              .Select(d => new { Id = (int)d, Name = d.ToString() })
                              .ToList();
        return Ok(departments);
    }
}
