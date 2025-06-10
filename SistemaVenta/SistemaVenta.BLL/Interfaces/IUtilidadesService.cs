namespace BLL.Interfaces;

public interface IUtilidadesService
{
    string ConvertirSHA256(string texto);
    string GenerarClave();
}
