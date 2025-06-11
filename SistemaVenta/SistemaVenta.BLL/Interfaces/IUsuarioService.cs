using Entity;

namespace BLL.Interfaces;

public interface IUsuarioService
{
    Task<bool> CambiarClave(int id, string claveActual, string nuevaClave);
    Task<Usuario> Crear(Usuario usuario,
                               Stream? foto = null,
                               string nombreFoto = "",
                               string UrlPlantillaCorreo = "");
    Task<Usuario> EditarUsuario(Usuario usuario,
                                Stream? foto = null,
                                string nombreFoto = "");
    Task<bool> EliminarUsuario(int id);
    Task<Usuario> GuardarUsuario(Usuario usuario);
    Task<List<Usuario>> ListarUsuarios();
    Task<Usuario> ObtenerUsuarioPorCredenciales(string correo, string clave);
    Task<Usuario> ObtenerUsuarioPorId(int id);
    Task<bool> RestablecerClave(string correo, string urlPlantilla = "");
}
