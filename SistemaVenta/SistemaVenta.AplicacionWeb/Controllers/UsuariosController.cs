using Microsoft.AspNetCore.Mvc;

namespace AplicacionWeb.Controllers;

public class UsuariosController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
