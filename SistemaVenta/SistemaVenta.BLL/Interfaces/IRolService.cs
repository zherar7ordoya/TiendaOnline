using Entity;

namespace BLL.Interfaces;

public interface IRolService
{
    Task<List<Rol>> ListarRoles();
}
