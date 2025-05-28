namespace BLL.Interfaces;

public interface ICorreoService
{
    Task<bool> EnviarCorreo(string destinatario, string asunto, string mensaje);
}