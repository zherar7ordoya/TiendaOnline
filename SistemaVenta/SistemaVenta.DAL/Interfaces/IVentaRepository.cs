using Entity;

namespace DAL.Interfaces;

public interface IVentaRepository : IGenericRepository<Venta>
{
    Task<List<DetalleVenta>> GenerarReporte(DateTime inicio, DateTime final);
    Task<Venta> Registrar(Venta venta);
}
