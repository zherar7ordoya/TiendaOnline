using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces;

public interface IFirebaseService
{
    Task<bool> EliminarStorage(string carpeta, string archivo);
    Task<string> SubirStorage(Stream stream, string carpeta, string archivo);
}
