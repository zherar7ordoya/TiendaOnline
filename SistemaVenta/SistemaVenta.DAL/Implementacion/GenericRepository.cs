using System.Linq.Expressions;

using DAL.DBContext;

using Microsoft.EntityFrameworkCore;

namespace DAL.Implementacion;

public class GenericRepository<TEntity>(DBVENTAContext context)
    : Interfaces.IGenericRepository<TEntity> where TEntity
    : class
{
    public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
    {
        IQueryable<TEntity> queryEntidad = filtro == null
            ? context.Set<TEntity>()
            : context.Set<TEntity>().Where(filtro);
        return queryEntidad;
    }

    public async Task<TEntity> Crear(TEntity entidad)
    {
        try
        {
            context.Add(entidad);
            await context.SaveChangesAsync();
            return entidad;
        }
        catch { throw; }
    }

    public async Task<bool> Editar(TEntity entidad)
    {
        try
        {
            context.Update(entidad);
            await context.SaveChangesAsync();
            return true;
        }
        catch { throw; }
    }

    public async Task<bool> Eliminar(TEntity entidad)
    {
        try
        {
            context.Remove(entidad);
            await context.SaveChangesAsync();
            return true;
        }
        catch { throw; }
    }

    public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
    {
        try
        {
            TEntity? entidad = await context.Set<TEntity>().FirstOrDefaultAsync(filtro);
            return entidad;
        }
        catch { throw; }
    }
}