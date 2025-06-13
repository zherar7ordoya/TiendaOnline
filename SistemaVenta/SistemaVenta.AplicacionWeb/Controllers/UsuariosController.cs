using AplicacionWeb.Models.ViewModels;
using AplicacionWeb.Utilidades.Response;

using AutoMapper;

using BLL.Interfaces;

using Entity;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Diagnostics;

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
    public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
    {
        GenericResponse<VMUsuario> gResponse = new();

        try
        {
            VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
            string nombreFoto = string.Empty;
            Stream? fotoStream = null;

            if (foto != null)
            {
                string nombre_en_codigo = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(foto.FileName);
                nombreFoto = string.Concat(nombre_en_codigo, extension);

                try
                {
                    EventLog.WriteEntry("Application", "Intentando abrir stream de foto...", EventLogEntryType.Information);
                    fotoStream = foto.OpenReadStream();
                }
                catch (Exception ex)
                {
                    System.IO.File.WriteAllText("error_log.txt", ex.ToString());
                    throw;
                }
            }

            string urlPlantillaCorreo =
                $"{Request.Scheme}://{Request.Host}/" +
                $"Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

            Usuario usuario_creado = await usuarioService.Crear
            (
                mapper.Map<Usuario>(vmUsuario),
                fotoStream,
                nombreFoto,
                urlPlantillaCorreo
            );

            vmUsuario = mapper.Map<VMUsuario>(usuario_creado);
            gResponse.Estado = true;
            gResponse.Objeto = vmUsuario;
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpPut]
    public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
    {
        GenericResponse<VMUsuario> gResponse = new();

        try
        {
            VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
            string nombreFoto = string.Empty;
            Stream? fotoStream = null;

            if (foto != null)
            {
                string nombre_en_codigo = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(foto.FileName);
                nombreFoto = string.Concat(nombre_en_codigo, extension);
                fotoStream = foto.OpenReadStream();
            }

            Usuario usuario_editado = await usuarioService.Editar
            (
                mapper.Map<Usuario>(vmUsuario),
                fotoStream,
                nombreFoto
            );

            vmUsuario = mapper.Map<VMUsuario>(usuario_editado);
            gResponse.Estado = true;
            gResponse.Objeto = vmUsuario;
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }
        return StatusCode(StatusCodes.Status200OK, gResponse);
    }

    [HttpDelete]
    public async Task<IActionResult> Eliminar(int IdUsuario)
    {
        GenericResponse<string> gResponse = new();
        try
        {
            gResponse.Estado = await usuarioService.Eliminar(IdUsuario);
        }
        catch (Exception ex)
        {
            gResponse.Estado = false;
            gResponse.Mensaje = ex.Message;
        }

        return StatusCode(StatusCodes.Status200OK, gResponse);
    }
}
