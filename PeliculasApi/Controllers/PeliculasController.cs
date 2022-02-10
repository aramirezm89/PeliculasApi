using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;
using PeliculasApi.Utils;
using System.Collections.Generic;
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

        public PeliculasController(IMapper mapper, ApplicationDbContext db, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.mapper = mapper;
            this._db = db;
            this.almacenadorArchivos = almacenadorArchivos;
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaDTO>>Get([FromRoute]int id)
        {
            var pelicula = await _db.Peliculas.FirstOrDefaultAsync(p => p.Id == id);  
            if (pelicula == null)
            {
                return NotFound();
            }
           var generos =  _db.PeliculasGeneros.Where( p => p.PeliculaId == pelicula.Id);
            return  mapper.Map<PeliculaDTO>(pelicula);
        }

        [HttpGet("generos/{id:int}")]
        public async Task<ActionResult<PeliculasPostGetDTO>>GetGeneros([FromRoute] int id)
        {
           
            var generos = await _db.PeliculasGeneros.Where(p => p.PeliculaId == id).ToListAsync();
            List<GeneroDTO> listaGenero = new List<GeneroDTO>();  
            foreach (var item in generos)
            {
                var nombreGenero = await  _db.Generos.FirstOrDefaultAsync(g => g.Id == item.GeneroId);
                listaGenero.Add(mapper.Map<GeneroDTO>(nombreGenero));  
            }
           

            return new PeliculasPostGetDTO() { Generos = listaGenero};
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {

            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);
            var peliculaRepetida = await _db.Peliculas.FirstOrDefaultAsync(p => p.Titulo == peliculaCreacionDTO.Titulo);
            if (peliculaRepetida == null)
            {
                if (peliculaCreacionDTO.Poster != null)
                {
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenedor, peliculaCreacionDTO.Poster);
                }

                EscribirOrdenActores(pelicula);

                _db.Add(pelicula); 
                await _db.SaveChangesAsync();
                var peliculaCreada = await _db.Peliculas.FirstOrDefaultAsync(p => p.Titulo == peliculaCreacionDTO.Titulo);
                return new JsonResult(new { succes = true, message = "Registro guardado.", code = 200,idPelicula = peliculaCreada.Id });
            }
            return new JsonResult(new { succes = false, message = "EL Titulo de la pelicula ya se encuentra en la base de datos.", code = 500 });

        }


        [HttpGet("postget")]
        /*metodo GET que devuelve eun listado de generos y cines con el fin de ser vidsualizados en el frontENd en el formulario
          de Creacion de Peliculas.
        */
        public async Task<ActionResult<PeliculasPostGetDTO>> PostGet()
        {
            var cines = await _db.Cines.ToListAsync();
            var generos =  await _db.Generos.ToListAsync();
            var cinesDTO = mapper.Map<List<CineDTO>>(cines);
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

            return new PeliculasPostGetDTO() { Generos = generosDTO , Cines = cinesDTO};
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
