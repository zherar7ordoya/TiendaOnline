using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using AplicacionWeb.Models.ViewModels;
using AplicacionWeb.Utilidades.Response;
using BLL.Interfaces;
using Entity;

namespace AplicacionWeb.Controllers;

public class UsuariosController
(
    IMapper mapper,
    IUsuarioService usuarioService,
    IRolService rolService
) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ListarRoles()
    {
        var lista = await rolService.ListarRoles();
        List<VMRol> vmListaRoles = mapper.Map<List<VMRol>>(lista);
        return StatusCode(StatusCodes.Status200OK, vmListaRoles);
    }

    [HttpGet]
    public async Task<IActionResult> ListarUsuarios()
    {
        var lista = await usuarioService.ListarUsuarios();
        List<VMUsuario> vmListaUsuarios = mapper.Map<List<VMUsuario>>(lista);
        return StatusCode(StatusCodes.Status200OK, new {data = vmListaUsuarios});
    }

    [HttpPost]
    public async Task<IActionResult> CrearUsuario([FromForm] IFormFile foto, [FromForm] string modelo)
    {
        GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

        try
        {
            VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
            string nombreFoto = string.Empty;
            if (foto != null)
            {
                string nombre_en_codigo = Guid.NewGuid().ToString("N");
            }
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;

        }
        catch (Exception)
        {

            throw;
        }
    }
}
