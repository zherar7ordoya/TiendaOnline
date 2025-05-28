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
    public async Task<bool> CambiarClave(int id, string claveActual, string nuevaClave)
    {
        try
        {
        }
        catch { throw; }
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

    public async Task<Usuario> EditarUsuario(Usuario usuario, Stream? foto = null, string nombreFoto = "")
    {
        Usuario existe = await repository.Obtener(x => x.Correo == usuario.Correo && x.IdUsuario != usuario.IdUsuario);
        if (existe != null) throw new TaskCanceledException("Ya existe un usuario con el correo proporcionado.");

        try
        {
            IQueryable<Usuario> query = await repository.Consultar(x => x.IdUsuario == usuario.IdUsuario);
            Usuario usuario_editar = query.First();
            usuario_editar.Nombre = usuario.Nombre;
            usuario_editar.Correo = usuario.Correo;
            usuario_editar.Telefono = usuario.Telefono;
            usuario_editar.IdRol = usuario.IdRol;

            if (usuario_editar.NombreFoto != nombreFoto)
            {
                usuario_editar.NombreFoto = nombreFoto;
            }

            if (foto != null)
            {
                string urlFoto = await firebase.SubirStorage(foto, "Carpeta_Usuario", usuario_editar.NombreFoto);
                usuario_editar.UrlFoto = urlFoto;
            }

            bool resultado = await repository.Editar(usuario_editar);

            if (!resultado) throw new TaskCanceledException("No se pudo editar el usuario.");

            Usuario usuario_editado = query.Include(rol => rol.IdRolNavigation).First();
            return usuario_editado;
        }
        catch { throw; }
    }

    public async Task<bool> EliminarUsuario(int id)
    {
        try
        {
            Usuario usuario_encontrado =
                await repository.Obtener(x => x.IdUsuario == id) ??
                throw new TaskCanceledException("No se encontró el usuario a eliminar.");

            string nombreFoto = usuario_encontrado.NombreFoto;
            bool resultado = await repository.Eliminar(usuario_encontrado);

            if (resultado) await firebase.EliminarStorage("Carpeta_Usuario", nombreFoto);
            return true;
        }
        catch { throw; }
    }

    public async Task<Usuario> GuardarUsuario(Usuario usuario)
    {
        try
        {
            Usuario usuario_encontrado = await repository.Obtener(x => x.IdUsuario == usuario.IdUsuario);
            if (usuario_encontrado == null) throw new TaskCanceledException("No se encontró el usuario a guardar.");

            usuario_encontrado.Correo = usuario.Correo;
            usuario_encontrado.Telefono = usuario.Telefono;

            bool resultado = await repository.Editar(usuario_encontrado);

            return resultado ? usuario_encontrado : throw new TaskCanceledException("No se pudo guardar el usuario.");
        }
        catch { throw; }
    }

    public async Task<List<Usuario>> ListarUsuarios()
    {
        IQueryable<Usuario> usuarios = await repository.Consultar();
        return usuarios.Include(rol => rol.IdRolNavigation).ToList();
    }

    public async Task<Usuario> ObtenerUsuarioPorCredenciales(string correo, string clave)
    {
        try
        {
            string clave_encriptada = utilidades.ConvertirSHA256(clave);
            Usuario usurario_encontrado = await repository.Obtener(x => x.Correo.Equals(correo) && x.Clave.Equals(clave_encriptada));
            return usurario_encontrado ?? throw new TaskCanceledException("No se encontró el usuario con las credenciales proporcionadas.");
        }
        catch { throw; }
    }

    public async Task<Usuario> ObtenerUsuarioPorId(int id)
    {
        try
        {
            IQueryable<Usuario> query = await repository.Consultar(x => x.IdUsuario == id);
            Usuario usuario_encontrado = query.Include(rol => rol.IdRolNavigation).FirstOrDefault() ??
                                          throw new TaskCanceledException("No se encontró el usuario con el ID proporcionado.");
            return usuario_encontrado;
        }
        catch { throw; }
    }

    public async Task<bool> RestablecerClave(string correo, string urlPlantilla = "")
    {
        try
        {
        }
        catch { throw; }
    }
}
