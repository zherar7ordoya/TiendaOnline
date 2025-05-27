using DAL.DBContext;
using DAL.Interfaces;

using Entity;

using Microsoft.EntityFrameworkCore;

namespace DAL.Implementacion;

public class VentaRepository(DBVENTAContext context)
    : GenericRepository<Venta>(context), IVentaRepository
{
    public async Task<List<DetalleVenta>> GenerarReporte(DateTime inicio, DateTime final)
    {
        // El include equivale a un join en SQL.
        List<DetalleVenta> listaVentas = await context.DetalleVenta
            .Include(venta => venta.IdVentaNavigation)
            .ThenInclude(usuario => usuario.IdUsuarioNavigation)
            .Include(venta => venta.IdVentaNavigation)
            .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
            .Where
            (
            detalleVenta => 
            detalleVenta.IdVentaNavigation.FechaRegistro.Value.Date >= inicio.Date &&
            detalleVenta.IdVentaNavigation.FechaRegistro.Value.Date <= final.Date
            )
            .ToListAsync();

        return listaVentas;
    }

    public async Task<Venta> Registrar(Venta venta)
    {
        Venta ventaGenerada = new();

        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                foreach (var detalle in venta.DetalleVenta)
                {
                    Producto productoEncontrado = context.Productos.Where(producto => producto.IdProducto == detalle.IdProducto).First();
                    productoEncontrado.Stock = productoEncontrado.Stock - detalle.Cantidad;
                    context.Productos.Update(productoEncontrado);
                }
                await context.SaveChangesAsync();
                NumeroCorrelativo correlativo = context.NumeroCorrelativos.Where(numero => numero.Gestion == "Venta").First();
                correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                correlativo.FechaActualizacion = DateTime.Now;
                context.NumeroCorrelativos.Update(correlativo);
                await context.SaveChangesAsync();

                string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);
                venta.NumeroVenta = numeroVenta;

                await context.Venta.AddAsync(venta);
                await context.SaveChangesAsync();
                ventaGenerada = venta;
                transaction.Commit();
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        return ventaGenerada;
    }
}
