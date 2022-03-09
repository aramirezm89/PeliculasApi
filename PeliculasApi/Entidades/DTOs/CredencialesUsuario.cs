using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.Entidades.DTOs
{
    public class CredencialesUsuario
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
