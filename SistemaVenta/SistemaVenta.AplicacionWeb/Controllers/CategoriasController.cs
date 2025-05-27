using Microsoft.AspNetCore.Mvc;

namespace AplicacionWeb.Controllers;

public class CategoriasController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
