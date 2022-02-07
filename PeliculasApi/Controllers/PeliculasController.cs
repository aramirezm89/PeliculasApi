using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;
using PeliculasApi.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext _db;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "peliculas";

        public PeliculasController(IMapper mapper, ApplicationDbContext db, IAlmacenadorArchivos almacenadorArchivos )
        {
            this.mapper = mapper;
            this._db = db;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {

            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);
            var peliculaRepetida = await _db.Peliculas.FirstOrDefaultAsync(p => p.Titulo == peliculaCreacionDTO.Titulo);
            if(peliculaRepetida == null)
            {
                if (peliculaCreacionDTO.Poster != null)
                {
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenedor, peliculaCreacionDTO.Poster);
                }

                EscribirOrdenActores(pelicula);

                _db.Add(pelicula);
                await _db.SaveChangesAsync();
                return new JsonResult(new { succes = true, message = "Registro guardado.", code = 200 });
            }
            return new JsonResult(new { succes = false, message = "EL Titulo de la pelicula ya se encuentra en la base de datos.", code = 500 });

        }

        private void EscribirOrdenActores(Pelicula pelicula)
        {
            if(pelicula.PeliculasActores != null)
            {
                pelicula.PeliculasActores.OrderBy(p => p.Orden);
            }
        }

    }
}
