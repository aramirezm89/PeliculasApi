using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;
using PeliculasApi.Utils;
using System;
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
        public async Task<ActionResult<PeliculaDTO>> Get( int id)
        {
            var pelicula = await _db.Peliculas
                .Include(x => x.PeliculasGenero).ThenInclude(x => x.Genero)
                .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                .Include(x => x.PeliculasCine).ThenInclude(x => x.Cine)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null) { return NotFound(); }

            var dto = mapper.Map<PeliculaDTO>(pelicula);
            dto.Actores =  dto.Actores.OrderBy(x => x.Orden).ToList();
            return dto;
        }
        [HttpGet("generos/{id:int}")]
        public async Task<ActionResult<List<GeneroDTO>>>GetGeneros([FromRoute] int id)
        {
           
            var generos = await _db.PeliculasGeneros.Where(p => p.PeliculaId == id).ToListAsync();
            List<GeneroDTO> listaGenero = new List<GeneroDTO>();  
            foreach (var item in generos)
            {
                var nombreGenero = await _db.Generos.FirstOrDefaultAsync(g => g.Id == item.GeneroId);
                listaGenero.Add(mapper.Map<GeneroDTO>(nombreGenero));  
            }
           

            return listaGenero;
        }

        [HttpGet]
        public async Task<ActionResult<LandingPageDTO>> GetLanding()
        {
            var top = 6;
            var hoy = DateTime.Today;

            var proximosEstrenos = await _db.Peliculas.Where(p => p.FechaLanzamiento > hoy)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();

            var enCines = await _db.Peliculas.
                          Where(p => p.EnCines)
                          .OrderBy(x => x.FechaLanzamiento)
                          .Take(6)
                          .ToListAsync();
            var resultado = new LandingPageDTO();

           resultado.ProximosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);   
           resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);   

            return resultado;   
        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] PeliculasFiltrarDTO peliculasFiltrarDTO)
        {
            var peliculasQueryable = _db.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(peliculasFiltrarDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.Titulo.Contains(peliculasFiltrarDTO.Titulo));
            }

            if (peliculasFiltrarDTO.Encines)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.EnCines);
            }

            if (peliculasFiltrarDTO.ProximosEstrenos)
            {
                peliculasQueryable = peliculasQueryable.Where(p  => p.FechaLanzamiento > DateTime.Today);
            }

            if(peliculasFiltrarDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.PeliculasGenero
                .Select(g => g.GeneroId).Contains(peliculasFiltrarDTO.GeneroId));
            }

            await HttpContext.InsertarParametrosEnCabecera(peliculasQueryable);

            var peliculas = await peliculasQueryable.Paginar(peliculasFiltrarDTO.PaginacionDTO).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);    
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

        [HttpGet("PutGet/{id:int}")]
        public async Task<ActionResult<PeliculasPutGetDTO>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult) { return NotFound(); }

            var pelicula = peliculaActionResult.Value;

            var generosSeleccionadosIds = pelicula.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await _db.Generos
                .Where(x => !generosSeleccionadosIds.Contains(x.Id))
                .ToListAsync();

            var cinesSeleccionadosIds = pelicula.Cines.Select(x => x.Id).ToList();
            var cinesNoSeleccionados = await _db.Cines
                .Where(x => !cinesSeleccionadosIds.Contains(x.Id))
                .ToListAsync();

            var generosNoSeleccionadosDTO = mapper.Map<List<GeneroDTO>>(generosNoSeleccionados);
            var cinesNoSeleccionadosDTO = mapper.Map<List<CineDTO>>(cinesNoSeleccionados);

            var respuesta = new PeliculasPutGetDTO();
            respuesta.Pelicula = pelicula;
            respuesta.GenerosSeleccionados = pelicula.Generos;
            respuesta.GenerosNoSeleccionados = generosNoSeleccionadosDTO;
            respuesta.CinesSeleccionados = pelicula.Cines;
            respuesta.CinesNoSeleccionados = cinesNoSeleccionadosDTO;
            respuesta.Actores = pelicula.Actores;
            return respuesta;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute]int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = await _db.Peliculas
                .Include(x => x.PeliculasActores)
                .Include(x => x.PeliculasGenero)
                .Include(x => x.PeliculasCine)
                .FirstOrDefaultAsync(x => x.Id == id);  

            if(pelicula == null)
            {
                return NotFound();
            }  
            pelicula = mapper.Map(peliculaCreacionDTO, pelicula);

            if(peliculaCreacionDTO.Poster != null)
            {
                pelicula.Poster = await almacenadorArchivos.EditarArchivo(contenedor, peliculaCreacionDTO.Poster, pelicula.Poster);
            }

            EscribirOrdenActores(pelicula);

            await _db.SaveChangesAsync();
            return new JsonResult(new { succes = true, message = "Registro actualizado.", code = 200 });
        }
        private void EscribirOrdenActores(Pelicula pelicula)
        {
            if(pelicula.PeliculasActores != null)
            {
                pelicula.PeliculasActores.OrderBy(p => p.Orden);
            }
        }
      
        [HttpDelete("{id:int}")]

        public async Task<ActionResult> Delete(int id)
        {
            var pelicula = await _db.Peliculas.FirstOrDefaultAsync(p => p.Id == id);

            if(pelicula == null)
            {
                return NotFound();
            }

            _db.Remove(pelicula);
            await _db.SaveChangesAsync();
            await almacenadorArchivos.BorrarArchivo(pelicula.Poster,contenedor);
            return new JsonResult(new { succes = false, message = "Registro eliminado", code = 200 });
        }

    }
}
