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

            var generosQueryable =  _db.Generos.AsQueryable();
            await HttpContext.InsertarParametrosEnCabecera(generosQueryable);
            var generos = await generosQueryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<GeneroDTO>>(generos); 
        }



        [HttpGet("{id:int}")]

        public async Task<ActionResult<GeneroDTO>> GetForId([FromRoute] int id)
        {
            var genero = await _db.Generos.FindAsync(id);
            return mapper.Map<GeneroDTO>(genero);
          
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
                 var genero = mapper.Map<Genero>(generoCreacionDTO);
                _db.Add(genero);
                await _db.SaveChangesAsync();
                return new JsonResult(new { succes=true, message="Registro guardado con exito.",code=200});
           
       
            
        }

        [HttpPut]
        public async Task< ActionResult >Put([FromBody] GeneroCreacionDTO generoCreacion,[FromRoute] int id)
        {
            var genero = await _db.Generos.FirstOrDefaultAsync(x => x.Id == id);
            genero.Nombre = generoCreacion.Nombre;
            _db.Update(genero);
            await _db.SaveChangesAsync();
            return new JsonResult(new { succes = true, message = "Registro actualziado con exito.", code = 200 });
        }

        [HttpDelete]

        public ActionResult Delete()
        {
            throw new NotImplementedException();

        }


    }
}
