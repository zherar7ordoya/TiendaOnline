using Microsoft.AspNetCore.Mvc;

namespace AplicacionWeb.Controllers;

public class ReporteVentasController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
