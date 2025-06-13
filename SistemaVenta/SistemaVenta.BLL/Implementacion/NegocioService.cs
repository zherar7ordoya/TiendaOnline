using BLL.Interfaces;

using DAL.Interfaces;

using Entity;

namespace BLL.Implementacion;

public class NegocioService
(
    IGenericRepository<Negocio> repositorio,
    IFirebaseService firebaseService
) : INegocioService
{
    public async Task<Negocio> Obtener()
    {
        try
        {
            Negocio negocio_encontrado = await repositorio.Obtener(x => x.IdNegocio == 1);
            return negocio_encontrado;
        }
        catch { throw; }
    }

    public async Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string NombreLogo = "")
    {
        try
        {
            Negocio negocio_encontrado = await repositorio.Obtener(x => x.IdNegocio == 1);
            negocio_encontrado.NumeroDocumento = entidad.NumeroDocumento;
            negocio_encontrado.Nombre = entidad.Nombre;
            negocio_encontrado.Correo = entidad.Correo;
            negocio_encontrado.Direccion = entidad.Direccion;
            negocio_encontrado.Telefono = entidad.Telefono;
            negocio_encontrado.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
            negocio_encontrado.SimboloMoneda = entidad.SimboloMoneda;

            negocio_encontrado.NombreLogo = NombreLogo == "" ? NombreLogo : negocio_encontrado.NombreLogo;
            if (Logo != null)
            {
                string urlLogo = await firebaseService.SubirStorage(Logo, "carpeta_logo", negocio_encontrado.NombreLogo);
                negocio_encontrado.UrlLogo = urlLogo;
            }

            await repositorio.Editar(negocio_encontrado);
            return negocio_encontrado;
        }
        catch { throw; }
    }
}
