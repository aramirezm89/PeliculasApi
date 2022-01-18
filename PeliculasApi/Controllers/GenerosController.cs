using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PeliculasApi.Entidades;
using System;
using System.Collections.Generic;
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

        public GenerosController(ILogger<GenerosController> logger,ApplicationDbContext db)
        {

            this.logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genero>>> Get()
        {

            return await _db.Generos.ToListAsync();
        }



        [HttpGet("{id:int}")]

        public async Task<ActionResult<Genero>> GetForId(int id)
        {

            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Genero genero)
        {
        
                _db.Add(genero);
                await _db.SaveChangesAsync();
                return new JsonResult(new { succes=true, message="Registro guardado con exito.",code=200});
           
       
            
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genero genero)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]

        public ActionResult Delete()
        {
            throw new NotImplementedException();

        }


    }
}
