using BLL.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Implementacion;

public class UtilidadesService : IUtilidadesService
{
    public string ConvertirSHA256(string texto)
    {
        StringBuilder sb = new();
        using (SHA256 hash = SHA256.Create())
        {
            Encoding enc = Encoding.UTF8;
            byte[] result = hash.ComputeHash(enc.GetBytes(texto));
            foreach (byte b in result)
            {
                sb.Append(b.ToString("x2")); // Convert each byte to a hexadecimal string
            }
        }
        return sb.ToString();
    }

    public string GenerarClave()
    {
        string clave = new Guid().ToString("N")[..6];
        return clave;
    }
}
