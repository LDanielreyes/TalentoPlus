using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

public class Worker : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}