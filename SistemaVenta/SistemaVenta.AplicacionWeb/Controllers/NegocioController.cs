using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;
using AplicacionWeb.Models.ViewModels;
using AplicacionWeb.Utilidades.Response;
using BLL.Interfaces;
using Entity;

namespace AplicacionWeb.Controllers;

public class NegocioController
(
    IMapper mapper,
    INegocioService negocioService
) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Obtener()
    {
        GenericResponse<VMNegocio> gResponse = new();

        try
        {
            VMNegocio vmNegocio = mapper.Map<VMNegocio>(await negocioService.Obtener());
            gResponse.Estado = true;
            gResponse.Objeto = vmNegocio;
        }
        catch (Exception ex) {
            gResponse.Estado = true;
            gResponse.Mensaje = ex.Message;
        }

        return StatusCode(200, gResponse);
    }

    [HttpPost]
    public async Task<IActionResult> GuardarCambios([FromForm] IFormFile logo, [FromForm] string modelo)
    {
        GenericResponse<VMNegocio> gResponse = new();

        try
        {
            VMNegocio vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);
            
            string nombreLogo = string.Empty;
            Stream logoStream = null;

            if (logo != null)
            {
                string nombre_en_codigo = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(logo.FileName);
                nombreLogo = string.Concat(nombre_en_codigo, extension);
                logoStream = logo.OpenReadStream();
            }

            Negocio negocio_editado = await negocioService.GuardarCambios(
                mapper.Map<Negocio>(vmNegocio),
                logoStream,
                nombreLogo
            );

            vmNegocio = mapper.Map<VMNegocio>(negocio_editado);

            gResponse.Estado = true;
            gResponse.Objeto = vmNegocio;
        }
        catch (Exception ex)
        {
            gResponse.Estado = true;
            gResponse.Mensaje = ex.Message;
        }

        return StatusCode(200, gResponse);
    }
}
