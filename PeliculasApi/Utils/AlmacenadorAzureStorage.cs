using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PeliculasApi.Utils
{
    public class AlmacenadorAzureStorage : IAlmacenadorArchivos
    {
        private string connectionString;
        public AlmacenadorAzureStorage(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage"); //esta cadena de conexion se encuentra en appsettings.json;
        }

        public async Task<string> GuardarArchivo(string contenedor, IFormFile archivo)
        {
            var cliente = new BlobContainerClient(connectionString, contenedor);

            await cliente.CreateIfNotExistsAsync();
            cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var extensionArchivo = Path.GetExtension(archivo.FileName); //se obtiene la extension del archivo sea (JPEG PNG) etc

            var nombreArchivo = $"{ Guid.NewGuid()}{extensionArchivo}"; // con guid le damos un identificador unico y le agregamos la extension del archivo recibido en la funcion

            var blob = cliente.GetBlobClient(nombreArchivo);

            await blob.UploadAsync(archivo.OpenReadStream());

            return blob.Uri.ToString();
        }

        public async Task BorrarArchivo(string ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }

            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            var nombreArchivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(nombreArchivo);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> EditarArchivo(string contenedor, IFormFile archivo, string ruta)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenedor, archivo);
        }
    }
}
