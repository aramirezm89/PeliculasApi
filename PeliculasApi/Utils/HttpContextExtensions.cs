using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasApi.Utils
{
    public static class HttpContextExtensions
    {
        /*
         * clase creada para poder decirle al frontEnd cuantos registros tiene la tabla en la base de datos
         * con el fin de usar la paginacion
         */


        /*
         * metodo de extension  primer parametro indica donde de va a aplicar y el segundo el IQueryable
         * que representa la tabla a la que se hara la consulta en este caso contar cuantos registros tiene
         */
        public static async Task InsertarParametrosEnCabecera<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadTotalRegistros", cantidad.ToString());
        }

    }
}
