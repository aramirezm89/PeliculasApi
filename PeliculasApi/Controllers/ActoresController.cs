using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PeliculasApi.Entidades.DTOs;
using System.Threading.Tasks;
using System.Linq;
using PeliculasApi.Entidades;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.Utils;
using System.Collections.Generic;

namespace PeliculasApi.Controllers
{
    [Route("api/actores")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDbContext db, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this._db = db;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]   
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            /*
             * la variable actoresQueryable contiene los elementos de la tabla actores y se utilizo
             * AsQueryable() ya que el metodo InsertarParametrosEnCabecera() recibe este tipo de dato como 
             * parametro.
             * Metodo InsertarParametrosEnCabecera: su logica se encuentra en la clase HttpContextExtensions la 
             * cual se utilizoo para hacen un metodo de   extension de HttpContext
             */
            var actoresQueryable = _db.Actores.AsQueryable();
            await HttpContext.InsertarParametrosEnCabecera(actoresQueryable);

            /*
             * La variable actores contiene un listado en el  cual se utiliza el metodo Paginar() el cual su logica
             * se encuentra en la clase IQueryableExtensions la cual recibe un objeto de tipo PaginacionDTO con el fin 
             * de paginar el resultado de la consulta.
             */
            var actores = await actoresQueryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();

            /*
             * El retorno es una lista de tipo actorDTO por lo cual se utiliza mapper para mapear la variable generos la
             * cual es de tipo Genero a GeneroDTO que es el tipo de dato que requiere como retorno la funcion.
             */
            return mapper.Map<List<ActorDTO>>(actores);
        }

        [HttpGet("{id:int}")]

        public async Task<ActionResult<ActorDTO>> Get([FromRoute] int id)
        {
            var actor = await _db.Actores.FirstOrDefaultAsync(g => g.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            return mapper.Map<ActorDTO>(actor);

        }
        [HttpGet("buscarPorNombre/{nombre}")]
        public async Task<ActionResult<List<PeliculaActorDTO>>>BuscarPorNombre([FromRoute]string nombre = "")
        {
            if (string.IsNullOrEmpty(nombre))
            {
                return new List<PeliculaActorDTO>();
            }

            return await _db.Actores
                .Where(a => a.Nombre.Contains(nombre)).OrderBy(a => a.Nombre)
                .Select(a => new PeliculaActorDTO { Id = a.Id, Nombre = a.Nombre, Foto = a.Foto })
                .Take(5)
                .ToListAsync();
        }
     

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actor = mapper.Map<Actor>(actorCreacionDTO);
            var validarActorRepetido = await  _db.Actores.FirstOrDefaultAsync(a => a.Nombre == actorCreacionDTO.Nombre);
           
            if (validarActorRepetido == null)
            {
               
                if (actorCreacionDTO.Foto != null)
                {
                  actor.Foto =  await almacenadorArchivos.GuardarArchivo(contenedor, actorCreacionDTO.Foto);
                }

                _db.Actores.Add(actor);
                await _db.SaveChangesAsync();
          
                return new JsonResult(new { succes = true, message = "Registro guardado.", code = 200 });
            }
            return new JsonResult(new { succes = false, message = "El Actor ya se encuentra en la base de datos.", code = 500 });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromForm] ActorCreacionDTO actorCreacionDTO, [FromRoute] int id)
            {
            var actor = await _db.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor == null)
            {
                return NotFound();
            }

            actor = mapper.Map(actorCreacionDTO, actor);

            if(actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorArchivos.EditarArchivo(contenedor, actorCreacionDTO.Foto, actor.Foto);
            }
            _db.Update(actor);
            await _db.SaveChangesAsync();
            return new JsonResult(new { succes = true, message = "Registro actualizado.", code = 200 });
        }

        [HttpDelete("{id:int}")]

        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            if (id == 0)
            {
                return new JsonResult(new { succes = false, message = "Error", code = 404 });
            }

            var actor = await _db.Actores.FirstOrDefaultAsync(g => g.Id == id);

            if (actor != null)
            {
                _db.Actores.Remove(actor);
                await _db.SaveChangesAsync();
                await almacenadorArchivos.BorrarArchivo(actor.Foto, contenedor);
                return new JsonResult(new { succes = false, message = "Registro eliminado", code = 200 });
            }
            else
            {
                return new JsonResult(new { succes = false, message = "El Registro que desea eliminar no existe en la base de datos.", code = 404 });
            }


        }

    }
}
