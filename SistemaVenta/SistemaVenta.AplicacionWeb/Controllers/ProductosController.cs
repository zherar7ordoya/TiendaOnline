using Microsoft.AspNetCore.Mvc;

namespace AplicacionWeb.Controllers;

public class ProductosController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
