namespace BLL.Interfaces;

public interface ICorreoService
{
    Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string mensaje);
}