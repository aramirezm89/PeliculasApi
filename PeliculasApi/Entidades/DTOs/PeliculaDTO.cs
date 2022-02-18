using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades.DTOs
{
    public class PeliculaDTO
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
        public List<GeneroDTO> Generos { get; set; }
        public List<PeliculaActorDTO> Actores { get; set; }
        public List<CineDTO> Cines { get; set; }

    }
}
