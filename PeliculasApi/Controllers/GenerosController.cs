using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;
using PeliculasApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasApi.Controllers
{
    [Route("api/generos")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenerosController : ControllerBase
    {


        private readonly ILogger<GenerosController> logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper mapper;

        public GenerosController(ILogger<GenerosController> logger,ApplicationDbContext db,IMapper mapper)
        {

            this.logger = logger;
            _db = db;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            /*
             * la variable generosQueryable contiene los elementos de la tabla Generos y se utilizo
             * AsQueryable() ya que el metodo InsertarParametrosEnCabecera() recibe este tipo de dato como 
             * parametro.
             * Metodo InsertarParametrosEnCabecera: su logica se encuentra en la clase HttpContextExtensions la 
             * cual se utilizoo para hacen un metodo de   extension de HttpContext
             */
            var generosQueryable =  _db.Generos.AsQueryable();
            await HttpContext.InsertarParametrosEnCabecera(generosQueryable);

            /*
             * La variable generos contiene un listado en el  cual se utiliza el metodo Paginar() el cual su logica
             * se encuentra en la clase IQueryableExtensions la cual recibe un objeto de tipo PaginacionDTO con el fin 
             * de paginar el resultado de la consulta.
             */
            var generos = await generosQueryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();

            /*
             * El retorno es una lista de tipo GeneroDTO por lo cual se utiliza mapper para mapear la variable generos la
             * cual es de tipo Genero a GeneroDTO que es el tipo de dato que requiere como retorno la funcion.
             */
            return mapper.Map<List<GeneroDTO>>(generos); 
        }



        [HttpGet("{id:int}")]

        public async Task<ActionResult<GeneroDTO>> Get([FromRoute] int id)
        {
            var genero = await _db.Generos.FirstOrDefaultAsync(g => g.Id == id);
            if(genero == null)
            {
                return NotFound();  
            }
            return mapper.Map<GeneroDTO>(genero);
          
        }

        [HttpGet("todos")]
        public async Task <ActionResult<List<GeneroDTO>>> TodosLosGeneros()
        {
            var generos = await _db.Generos.OrderBy(g => g.Nombre).ToListAsync();

            return mapper.Map<List<GeneroDTO>>(generos);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var ValidarGeneroRepetido = await _db.Generos.FirstOrDefaultAsync(g => g.Nombre == generoCreacionDTO.Nombre);

            if(ValidarGeneroRepetido == null)
            {
                var genero = mapper.Map<Genero>(generoCreacionDTO);
                _db.Add(genero);
                await _db.SaveChangesAsync();
                return new JsonResult(new { succes = true, message = "Registro guardado.", code = 200 });

            }

            return new JsonResult(new { succes = false, message = "El  genero ya se encuentra en la base de datos.", code = 500 });

        }

        [HttpPut("{id:int}")]
        public async Task< ActionResult >Put([FromBody] GeneroCreacionDTO generoCreacion,[FromRoute] int id)
        {
            var genero = await _db.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if(genero == null)
            {
                return NotFound();
            }
            genero.Nombre = generoCreacion.Nombre;
            _db.Update(genero);
            await _db.SaveChangesAsync();
            return new JsonResult(new { succes = true, message = "Registro actualizado.", code = 200 });
        }

        [HttpDelete("{id:int}")]

        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            if(id == 0)
            {
                return new JsonResult(new { succes = false, message = "Error", code = 404 });
            }

            var genero = await _db.Generos.FirstOrDefaultAsync(g => g.Id == id);

            if(genero != null)
            {
                _db.Generos.Remove(genero);
               await _db.SaveChangesAsync();
                return new JsonResult(new { succes = false, message = "Registro eliminado", code = 200 });
            }
            else
            {
                return new JsonResult(new { succes = false, message = "El registro que desea eliminar no existe en la base de datos.", code = 404 });
            }

           
        }


    }
}
