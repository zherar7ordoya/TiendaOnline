using Microsoft.AspNetCore.Mvc;

namespace AplicacionWeb.Controllers;

public class NegocioController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
