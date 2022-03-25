using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        public string Titulo { get; set; }
        public string Resumen { get; set; }
        public string Trailer { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string Poster { get; set; }

        public List<PeliculasActores> PeliculasActores { get; set; }
        public List<PeliculasGenero> PeliculasGenero { get; set; }
        public List<PeliculasCines> PeliculasCine { get; set; }

    }
}
