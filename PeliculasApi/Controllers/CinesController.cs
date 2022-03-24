using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using PeliculasApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/cines")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Policy = "EsAdmin")]
    public class CinesController : ControllerBase   
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper mapper;

        public CinesController(ApplicationDbContext db, IMapper mapper)
        {
            this._db = db;
            this.mapper = mapper;
        }

        [HttpGet]

        public async Task<ActionResult<List<CineDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            /*
            * la variable cinesQueryable contiene los elementos de la tabla CIne y se utilizo
            * AsQueryable() ya que el metodo InsertarParametrosEnCabecera() recibe este tipo de dato como 
            * parametro.
            * Metodo InsertarParametrosEnCabecera: su logica se encuentra en la clase HttpContextExtensions la 
            * cual se utilizoo para hacen un metodo de   extension de HttpContext
            */
            var cinesQueryable = _db.Cines.AsQueryable();
            await HttpContext.InsertarParametrosEnCabecera(cinesQueryable);

            /*
             * La variable generos contiene un listado en el  cual se utiliza el metodo Paginar() el cual su logica
             * se encuentra en la clase IQueryableExtensions la cual recibe un objeto de tipo PaginacionDTO con el fin 
             * de paginar el resultado de la consulta.
             */
            var cines = await cinesQueryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
        

            /*
             * El retorno es una lista de tipo GeneroDTO por lo cual se utiliza mapper para mapear la variable generos la
             * cual es de tipo Genero a GeneroDTO que es el tipo de dato que requiere como retorno la funcion.
             */
            return mapper.Map<List<CineDTO>>(cines);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CineDTO>> Get([FromRoute] int id)
        {
            var cine = await _db.Cines.FirstOrDefaultAsync(c => c.Id == id);    
            if(cine == null)
            {
                return NotFound();
            }


            return mapper.Map<CineDTO>(cine);
           
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CineCreacionDTO cineCreacionDTO)
        {
            var validarNombreRepetido = await _db.Cines.FirstOrDefaultAsync(c => c.Nombre == cineCreacionDTO.Nombre);
            if(validarNombreRepetido == null)
            {
                var cine = mapper.Map<Cine>(cineCreacionDTO);
                _db.Add(cine);
                await _db.SaveChangesAsync();
                return new JsonResult(new { succes = true, message = "Registro guardado.", code = 200 });

            }

            return new JsonResult(new { succes = false, message = "El  genero ya se encuentra en la base de datos.", code = 500 });

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute]int id, [FromBody] CineCreacionDTO cineDTO)
        {
            var cineBd = await _db.Cines.FirstOrDefaultAsync(c => c.Id == id);

            if(cineBd == null)
            {
                return NotFound();
            }

            var cine =   mapper.Map(cineDTO,cineBd);
            _db.Update(cine);
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

            var cine = await _db.Cines.FirstOrDefaultAsync(c => c.Id == id);

            if(cine != null)
            {
               _db.Remove(cine);
                await _db.SaveChangesAsync();
                return new JsonResult(new { succes = true, message = "Registro eliminado", code = 200 });
            }
            else
            {
                return new JsonResult(new { succes = false, message = "El registro que desea eliminar no existe en la base de datos.", code = 404 });
            }
        }

        
    }
}
