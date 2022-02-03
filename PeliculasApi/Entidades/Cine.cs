using NetTopologySuite.Geometries;
using PeliculasApi.Entidades.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades
{
    public class Cine
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:50)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public Point Ubicacion { get; set; }    

    }
}
