using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BLL.Interfaces;
using BLL.Implementacion;
using DAL.DBContext;
using DAL.Implementacion;
using DAL.Interfaces;

namespace IoC;

public static class Dependencia
{
    public static async Task InyectarDependenciaAsync(this IServiceCollection services)
    {
        var vaultName = "zherar7ordoya";
        var vaultUri = $"https://{vaultName}.vault.azure.net/";
        var secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());

        KeyVaultSecret kvSecret = await secretClient.GetSecretAsync("ConnectionStringDBVENTA");
        string connectionString = kvSecret.Value;

        services.AddDbContext<DBVENTAContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<ICorreoService, CorreoService>();
        services.AddScoped<IFirebaseService, FirebaseService>();
        services.AddScoped<IUtilidadesService, UtilidadesService>();
        services.AddScoped<IRolService, RolService>();
    }
}