using Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces;

public interface IUsuarioService
{
    Task<bool> CambiarClave(int id, string claveActual, string nuevaClave);
    Task<Usuario> CrearUsuario(Usuario usuario,
                               Stream? foto = null,
                               string nombreFoto = "",
                               string urlPlantilla = "");
    Task<Usuario> EditarUsuario(Usuario usuario,
                                Stream? foto = null,
                                string nombreFoto = "");
    Task<Usuario> EliminarUsuario(int id);
    Task<Usuario> GuardarUsuario(Usuario usuario);
    Task<List<Usuario>> ListarUsuarios();
    Task<Usuario> ObtenerUsuarioPorCredenciales(string correo, string clave);
    Task<Usuario> ObtenerUsuarioPorId(int id);
    Task<bool> RestablecerClave(string correo, string urlPlantilla = "");
}
