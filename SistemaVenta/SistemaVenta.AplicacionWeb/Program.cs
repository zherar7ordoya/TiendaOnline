using IoC;

using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using AplicacionWeb.Utilidades.AutoMapper;


/* |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| */

//ProbarEmail(); // This is a test to send an email using Zoho SMTP in C#.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
await builder.Services.InyectarDependenciaAsync();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

/* |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| */

//async static void ProbarEmail()
//{
//    string host = "smtp.zoho.com";
//    int port = 587;
//    string correo = "zherar7ordoya@zohomail.com";
//    string clave = "ZjvuZ4pWRa34";
//    string alias = "Sistema de venta";

//    // Pon un correo donde quieras recibir la prueba.
//    string destinatario = "gerardo.tordoya@outlook.com";
//    string asunto = "Prueba de correo desde Zoho SMTP";
//    string cuerpo = "<h1>Hola, Gerardo!</h1><p>Esta es una prueba de enviado de correo desde Zoho SMTP usando C#.</p>";

//    try
//    {
//        var credenciales = new NetworkCredential(correo, clave);

//        using var cliente = new SmtpClient();
//        cliente.Host = host;
//        cliente.Port = port;
//        cliente.EnableSsl = true;
//        cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
//        cliente.UseDefaultCredentials = false;
//        cliente.Credentials = credenciales;

//        var mail = new MailMessage()
//        {
//            From = new MailAddress(correo, alias),
//            Subject = asunto,
//            Body = cuerpo,
//            IsBodyHtml = true
//        };

//        mail.To.Add(destinatario);

//        await cliente.SendMailAsync(mail);

//        Debug.WriteLine("Correo enviado exitosamente.");
//    }
//    catch (Exception ex)
//    {
//        Debug.WriteLine($"Error enviando correo: {ex.Message}");
//    }
//}