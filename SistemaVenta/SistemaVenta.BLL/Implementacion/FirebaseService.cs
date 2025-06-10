using BLL.Interfaces;

using DAL.Interfaces;

using Entity;

using Firebase.Auth;
using Firebase.Storage;

namespace BLL.Implementacion;

public class FirebaseService(IGenericRepository<Configuracion> repository) : IFirebaseService
{
    public async Task<bool> EliminarStorage(string carpeta, string archivo)
    {
        try
        {
            IQueryable<Configuracion> query = await repository.Consultar(c => c.Recurso.Equals("Firebase_Storage"));
            Dictionary<string, string> configuraciones = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(configuraciones["API_Key"]));
            var a = await auth.SignInWithEmailAndPasswordAsync(configuraciones["Email"], configuraciones["Clave"]);
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage
                (configuraciones["Ruta"],
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(carpeta)
                .Child(archivo)
                .DeleteAsync();

            await task;
            return true;
        }
        catch { return false; }
    }

    public async Task<string> SubirStorage(Stream stream, string carpeta, string archivo)
    {
        string url = string.Empty;

        try
        {
            IQueryable<Configuracion> query = await repository.Consultar(c => c.Recurso.Equals("Firebase_Storage"));
            Dictionary<string, string> configuraciones = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

            var auth = new FirebaseAuthProvider(new FirebaseConfig(configuraciones["API_Key"]));
            var a = await auth.SignInWithEmailAndPasswordAsync(configuraciones["Email"], configuraciones["Clave"]);
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage
                (configuraciones["Ruta"],
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(carpeta)
                .Child(archivo)
                .PutAsync(stream, cancellation.Token);

            url = await task;
        }
        catch { url = string.Empty; }

        return url;
    }
}
