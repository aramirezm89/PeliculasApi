using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PeliculasApi.Utils
{
    //IAlmacenadorArchivos es una interfaz creada a partir de la clase AlmacenadorAzureStorage
    public interface IAlmacenadorArchivos
    {
        Task BorrarArchivo(string ruta, string contenedor);
        Task<string> EditarArchivo(string contenedor, IFormFile archivo, string ruta);
        Task<string> GuardarArchivo(string contenedor, IFormFile archivo);
    }
}