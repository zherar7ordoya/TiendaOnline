using BLL.Interfaces;

using DAL.Interfaces;

using Entity;

using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;

namespace BLL.Implementacion;

public class UsuarioService
(
    IGenericRepository<Usuario> genericRepository,
    IFirebaseService firebaseService,
    IUtilidadesService utilidadesService,
    ICorreoService correoService
) : IUsuarioService
{
    public async Task<bool> CambiarClave(int id, string claveActual, string nuevaClave)
    {
        try
        {
            Usuario usuario_encontrado = await genericRepository.Obtener(x => x.IdUsuario == id);
            if (usuario_encontrado == null) throw new TaskCanceledException("No se encontró el usuario a cambiar la clave.");

            string clave_actual_encriptada = utilidadesService.ConvertirSHA256(claveActual);
            if (usuario_encontrado.Clave != clave_actual_encriptada) throw new TaskCanceledException("La clave actual proporcionada no es correcta.");

            string nueva_clave_encriptada = utilidadesService.ConvertirSHA256(nuevaClave);
            usuario_encontrado.Clave = nueva_clave_encriptada;

            bool resultado = await genericRepository.Editar(usuario_encontrado);
            if (!resultado) throw new TaskCanceledException("No se pudo cambiar la clave del usuario.");
            return resultado;
        }
        catch { throw; }
    }

    public async Task<Usuario> CrearUsuario(Usuario usuario, Stream? foto = null, string nombreFoto = "", string urlPlantilla = "")
    {
        Usuario existe = await genericRepository.Obtener(x => x.Correo == usuario.Correo);
        if (existe != null) throw new TaskCanceledException("Ya existe un usuario con el correo proporcionado.");

        try
        {
            string clave = utilidadesService.GenerarClave();
            usuario.Clave = utilidadesService.ConvertirSHA256(clave);
            usuario.NombreFoto = nombreFoto;

            if (foto != null)
            {
                string urlFoto = await firebaseService.SubirStorage(foto, "Carpeta_Usuario", nombreFoto);
                usuario.UrlFoto = urlFoto;
            }

            Usuario usuario_creado = await genericRepository.Crear(usuario);

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
                    await correoService.EnviarCorreo(usuario_creado.Correo, "Bienvenido al sistema de ventas", htmlCorreo);
                }
            }

            IQueryable<Usuario> query = await genericRepository.Consultar(x => x.IdUsuario == usuario_creado.IdUsuario);
            usuario_creado = query.Include(rol => rol.IdRolNavigation).First();
            return usuario_creado;
        }
        catch (Exception ex) { throw; }
    }

    public async Task<Usuario> EditarUsuario(Usuario usuario, Stream? foto = null, string nombreFoto = "")
    {
        Usuario existe = await genericRepository.Obtener(x => x.Correo == usuario.Correo && x.IdUsuario != usuario.IdUsuario);
        if (existe != null) throw new TaskCanceledException("Ya existe un usuario con el correo proporcionado.");

        try
        {
            IQueryable<Usuario> query = await genericRepository.Consultar(x => x.IdUsuario == usuario.IdUsuario);
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
                string urlFoto = await firebaseService.SubirStorage(foto, "Carpeta_Usuario", usuario_editar.NombreFoto);
                usuario_editar.UrlFoto = urlFoto;
            }

            bool resultado = await genericRepository.Editar(usuario_editar);

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
                await genericRepository.Obtener(x => x.IdUsuario == id) ??
                throw new TaskCanceledException("No se encontró el usuario a eliminar.");

            string nombreFoto = usuario_encontrado.NombreFoto;
            bool resultado = await genericRepository.Eliminar(usuario_encontrado);

            if (resultado) await firebaseService.EliminarStorage("Carpeta_Usuario", nombreFoto);
            return true;
        }
        catch { throw; }
    }

    public async Task<Usuario> GuardarUsuario(Usuario usuario)
    {
        try
        {
            Usuario usuario_encontrado = await genericRepository.Obtener(x => x.IdUsuario == usuario.IdUsuario);
            if (usuario_encontrado == null) throw new TaskCanceledException("No se encontró el usuario a guardar.");

            usuario_encontrado.Correo = usuario.Correo;
            usuario_encontrado.Telefono = usuario.Telefono;

            bool resultado = await genericRepository.Editar(usuario_encontrado);

            return resultado ? usuario_encontrado : throw new TaskCanceledException("No se pudo guardar el usuario.");
        }
        catch { throw; }
    }

    public async Task<List<Usuario>> ListarUsuarios()
    {
        IQueryable<Usuario> usuarios = await genericRepository.Consultar();
        return usuarios.Include(rol => rol.IdRolNavigation).ToList();
    }

    public async Task<Usuario> ObtenerUsuarioPorCredenciales(string correo, string clave)
    {
        try
        {
            string clave_encriptada = utilidadesService.ConvertirSHA256(clave);
            Usuario usurario_encontrado = await genericRepository.Obtener(x => x.Correo.Equals(correo) && x.Clave.Equals(clave_encriptada));
            return usurario_encontrado ?? throw new TaskCanceledException("No se encontró el usuario con las credenciales proporcionadas.");
        }
        catch { throw; }
    }

    public async Task<Usuario> ObtenerUsuarioPorId(int id)
    {
        try
        {
            IQueryable<Usuario> query = await genericRepository.Consultar(x => x.IdUsuario == id);
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
            Usuario usuario_encontrado = await genericRepository.Obtener(x => x.Correo == correo);
            if (usuario_encontrado == null) throw new TaskCanceledException("No se encontró el usuario con el correo proporcionado.");
            
            string clave_generada = utilidadesService.GenerarClave();
            usuario_encontrado.Clave = utilidadesService.ConvertirSHA256(clave_generada);

            bool resultado = await genericRepository.Editar(usuario_encontrado);
            
            if (!resultado) throw new TaskCanceledException("No se pudo restablecer la clave del usuario.");

            urlPlantilla = urlPlantilla.Replace("[Clave]", clave_generada);

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

            bool correo_enviado = false;
            if (htmlCorreo != "") correo_enviado = await correoService.EnviarCorreo(correo, "Restablecimiento de clave", htmlCorreo);
            if (!correo_enviado) throw new TaskCanceledException("No se pudo enviar el correo de restablecimiento de clave.");

            bool respuesta = await genericRepository.Editar(usuario_encontrado);
            if (!respuesta) throw new TaskCanceledException("No se pudo actualizar la clave del usuario en la base de datos.");
            return respuesta;

        }
        catch { throw; }
    }
}
