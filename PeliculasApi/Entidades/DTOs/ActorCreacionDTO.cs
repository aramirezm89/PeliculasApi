using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades.DTOs
{
    public class ActorCreacionDTO
    {
      
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength: 200)]
        public string Nombre { get; set; }
        public string Biografia { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public IFormFile Foto { get; set; }

    }
}
