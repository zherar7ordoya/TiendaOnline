using Microsoft.AspNetCore.Mvc;

namespace AplicacionWeb.Controllers;

public class HistorialVentasController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
