using System.Linq.Expressions;

namespace DAL.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>>? filtro = null);
    Task<TEntity> Crear(TEntity entidad);
    Task<bool> Editar(TEntity entidad);
    Task<bool> Eliminar(TEntity entidad);
    Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro);
}