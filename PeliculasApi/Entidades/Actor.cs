using PeliculasApi.Entidades.Validaciones;
using System;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades
{
    public class Actor
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido.")]
        [StringLength(maximumLength:200)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public string Biografia { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Foto { get; set; }


    }
}
