using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PeliculasApi.Utils;
using System;
using System.Collections.Generic;

namespace PeliculasApi.Entidades.DTOs
{
    public class PeliculaCreacionDTO
    {

        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public IFormFile Poster { get; set; } //Reicebe archivo
        /*para poder recibir un listado en este caso de generos,cines,y actores desde el FromForm realizamos un BinderType custom el cual
         * ayuda al modelBinder a recibir los datos del FromForm
         */

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> CinesIds { get; set; }
        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculaCreacionDTO>>))]
        public List<ActorPeliculaCreacionDTO> Actores { get; set; }
    }
}
