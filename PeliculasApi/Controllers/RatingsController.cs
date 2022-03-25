using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasApi.Controllers
{
    [Route("api/rating")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> userManager;

        public RatingsController(IMapper mapper, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            this.mapper = mapper;
            this._db = db;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //este endpoint no utiliza policy ya que un usuario que no sea admin tambien podra votar por la pelicula. solo necesita estar logueado
        public async Task<ActionResult> Post([FromBody] RatingDTO ratingDTO)
        {
            var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email").Value; //obtiene email del usuario por medio del claim  
            var usuario = await userManager.FindByEmailAsync(email);  //contiene el usuario de la base de dato que contiene el email.
            var usuarioId = usuario.Id; //contiene la id del usuario.

            var ratingActual = await _db.Ratings.FirstOrDefaultAsync(p => p.PeliculaId == ratingDTO.PeliculaId && p.UsuarioId == usuarioId);

            if (ratingActual == null)
            {
                var rating = new Rating();
                rating.PeliculaId = ratingDTO.PeliculaId;
                rating.Puntuacion = ratingDTO.Puntuacion;
                rating.UsuarioId = usuarioId;
                _db.Ratings.Add(rating);
            }
            else
            {
                ratingActual.Puntuacion = ratingDTO.Puntuacion;
            }

            await _db.SaveChangesAsync();

            return new JsonResult(new { code = 200, message = "Voto Realizado", success = true });

        }
    }
}
