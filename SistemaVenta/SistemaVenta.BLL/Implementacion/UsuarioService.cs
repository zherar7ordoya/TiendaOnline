using BLL.Interfaces;

using DAL.Interfaces;

using Entity;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Implementacion;

public class UsuarioService
(
    IGenericRepository<Usuario> repository,
    IFirebaseService firebase,
    IUtilidadesService utilidades,
    ICorreoService correo
) : IUsuarioService
{
    public Task<bool> CambiarClave(int id, string claveActual, string nuevaClave)
    {
        throw new NotImplementedException();
    }

    public async Task<Usuario> CrearUsuario(Usuario usuario, Stream? foto = null, string nombreFoto = "", string urlPlantilla = "")
    {
        Usuario existe = await repository.Obtener(x => x.Correo == usuario.Correo);
        if (existe != null) throw new TaskCanceledException("Ya existe un usuario con el correo proporcionado.");

        try
        {
            string clave = utilidades.GenerarClave();
            usuario.Clave = utilidades.ConvertirSHA256(clave);
            usuario.NombreFoto = nombreFoto;

            if (foto != null)
            {
                string urlFoto = await firebase.SubirStorage(foto, "Carpeta_Usuario", nombreFoto);
                usuario.UrlFoto = urlFoto;
            }

            Usuario usuario_creado = await repository.Crear(usuario);

            if (usuario_creado.IdUsuario == 0) throw new TaskCanceledException("No se pudo crear el usuario.");

            if (urlPlantilla != "")
            {
                urlPlantilla = urlPlantilla.Replace("[Correo]", usuario.Correo)
                                           .Replace("[Clave]", clave);

                string htmlCorreo = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantilla);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using Stream stream = response.GetResponseStream();
                    StreamReader reader = null;

                    if (response.CharacterSet == null)
                    {
                        reader = new StreamReader(stream);
                    }
                    else
                    {
                        reader = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet));
                    }

                    htmlCorreo = reader.ReadToEnd();

                    response.Close();
                    reader.Close();
                }
                if (htmlCorreo != "")
                {
                    await correo.EnviarCorreo(usuario_creado.Correo, "Bienvenido al sistema de ventas", htmlCorreo);
                }
            }

            IQueryable<Usuario> query = await repository.Consultar(x => x.IdUsuario == usuario_creado.IdUsuario);
            usuario_creado = query.Include(rol => rol.IdRolNavigation).First();
            return usuario_creado;
        }
        catch (Exception ex) { throw; }
    }

    public Task<Usuario> EditarUsuario(Usuario usuario, Stream? foto = null, string nombreFoto = "")
    {
        throw new NotImplementedException();
    }

    public Task<Usuario> EliminarUsuario(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Usuario> GuardarUsuario(Usuario usuario)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Usuario>> ListarUsuarios()
    {
        IQueryable<Usuario> usuarios = await repository.Consultar();
        return usuarios.Include(rol => rol.IdRolNavigation).ToList();
    }

    public Task<Usuario> ObtenerUsuarioPorCredenciales(string correo, string clave)
    {
        throw new NotImplementedException();
    }

    public Task<Usuario> ObtenerUsuarioPorId(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RestablecerClave(string correo, string urlPlantilla = "")
    {
        throw new NotImplementedException();
    }
}
