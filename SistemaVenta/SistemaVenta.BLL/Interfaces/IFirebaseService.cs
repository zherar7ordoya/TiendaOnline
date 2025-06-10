namespace BLL.Interfaces;

public interface IFirebaseService
{
    Task<bool> EliminarStorage(string carpeta, string archivo);
    Task<string> SubirStorage(Stream stream, string carpeta, string archivo);
}
