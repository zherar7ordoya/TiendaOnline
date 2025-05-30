using BLL.Interfaces;

using DAL.Interfaces;

using Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Implementacion;

public class RolService(IGenericRepository<Rol> repository) : IRolService
{
    public async Task<List<Rol>> ListarRoles()
    {
        IQueryable<Rol> query = await repository.Consultar();
        return query.ToList();
    }
}



