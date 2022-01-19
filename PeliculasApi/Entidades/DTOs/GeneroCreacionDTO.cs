using PeliculasApi.Entidades.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades.DTOs
{
    public class GeneroCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "El minimo de caracteres para el campo {0} es 2 y el maximo 10.")]
        [PrimeraLetraMayuscula]
        [Display(Name = "NombreGenero")]
        public string Nombre { get; set; }
    }
}
