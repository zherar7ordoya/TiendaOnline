using BLL.Interfaces;

using DAL.Interfaces;

using Entity;


using System.Net;
using System.Net.Mail;

namespace BLL.Implementacion;

public class CorreoService(IGenericRepository<Configuracion> repository) : ICorreoService
{
    public async Task<bool> EnviarCorreo(string destinatario, string asunto, string mensaje)
    {
        try
        {
            IQueryable<Configuracion> query = await repository.Consultar(c => c.Recurso.Equals("Servicio_Correo"));
            Dictionary<string, string> configuraciones = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);
            var credenciales = new NetworkCredential(configuraciones["Correo"], configuraciones["Clave"]);
            var correo = new MailMessage
            {
                From = new MailAddress(configuraciones["Correo"], configuraciones["Alias"]),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true
            };

            correo.To.Add(new MailAddress(destinatario));

            var cliente = new SmtpClient()
            {
                Host = configuraciones["Host"],
                Port = int.Parse(configuraciones["Puerto"]),
                Credentials = credenciales,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true
            };

            cliente.Send(correo);
            return true;
        }
        catch { return false; }
    }
}
