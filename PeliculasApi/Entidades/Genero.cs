using PeliculasApi.Entidades.Validaciones;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PeliculasApi.Entidades
{
    public class Genero
    {
      
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "El minimo de caracteres para el campo {0} es 2 y el maximo 10.")]
        [PrimeraLetraMayuscula]
        [Display(Name ="NombreGenero")]
        public string Nombre { get; set; }
        public List<PeliculasGenero> PeliculasGenero { get; set; }


    }
}
