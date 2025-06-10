using IoC;
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