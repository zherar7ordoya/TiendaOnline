using Entity;

namespace BLL.Interfaces;

public interface IUsuarioService
{
    Task<bool> CambiarClave(int id, string claveActual, string nuevaClave);
    Task<Usuario> Crear(Usuario usuario,
                               Stream? foto = null,
                               string nombreFoto = "",
                               string UrlPlantillaCorreo = "");
    Task<Usuario> Editar(Usuario usuario,
                                Stream? foto = null,
                                string nombreFoto = "");
    Task<bool> Eliminar(int id);
    Task<Usuario> Guardar(Usuario usuario);
    Task<List<Usuario>> ListarUsuarios();
    Task<Usuario> ObtenerUsuarioPorCredenciales(string correo, string clave);
    Task<Usuario> ObtenerUsuarioPorId(int id);
    Task<bool> RestablecerClave(string correo, string urlPlantilla = "");
}
