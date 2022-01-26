﻿using PeliculasApi.Entidades.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasApi.Utils
{
    public static class IQueryableExtensions
    {
        /*
       * metodo de extension para IQueryable  primer parametro indica donde de va a aplicar y el segundo el IQueryable
       * que representa la tabla a la que se hara la consulta en este caso contar cuantos registros tiene
       */
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable,PaginacionDTO paginacionDTO)
        {
            return queryable.Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina).Take(paginacionDTO.RecordsPorPagina);
        }
    }
}