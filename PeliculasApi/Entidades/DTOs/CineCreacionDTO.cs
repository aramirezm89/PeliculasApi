using NetTopologySuite.Geometries;
using PeliculasApi.Entidades.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades.DTOs
{
    public class CineCreacionDTO
    {
        [Required]
        [StringLength(maximumLength: 50)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        [Range(-90,90)]
        public double Latitud { get; set; }
        [Range(-180,180)]
        public double Longitud { get; set; }
    }

    //Las propiedas Latitud y Longitud no se encuentras en la clase Cine (La clase cine tiene una propiedad llamada Ubicacion de tipo Point)
    //con Automapper se mapeara las propiedades latitud y longitud recibidas desde el frontEnd a la propiedad Ubicacion de tipo Point
}
