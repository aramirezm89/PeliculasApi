﻿using System.Collections.Generic;

namespace PeliculasApi.Entidades.DTOs
{
    public class PeliculasPutGetDTO
    {
        public PeliculaDTO Pelicula { get; set; }
        public List<GeneroDTO> GenerosSeleccionados { get; set; }
        public List<GeneroDTO> GenerosNoSeleccionados { get; set; }
        public List<CineDTO> CinesSeleccionados { get; set; }
        public List<CineDTO> CinesNoSeleccionados { get; set; }
        public List<PeliculaActorDTO> Actores { get; set; }


    }
}
