using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PeliculasApi.Entidades.DTOs;
using System.Threading.Tasks;
using System.Linq;
using PeliculasApi.Entidades;
using Microsoft.EntityFrameworkCore;

namespace PeliculasApi.Controllers
{
    [Route("api/actores")]
    [ApiController]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper mapper;

        public ActoresController(ApplicationDbContext db, IMapper mapper)
        {
            this._db = db;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ActorCreacionDTO actorCreacionDTO)
        {
            var validarActorRepetido = await  _db.Actores.FirstOrDefaultAsync(a => a.Nombre == actorCreacionDTO.Nombre);    
            if (validarActorRepetido == null)
            {
                var actor = mapper.Map<Actor>(actorCreacionDTO);

                _db.Actores.Add(actor);
               await _db.SaveChangesAsync();
                return new JsonResult(new { succes = true, message = "Registro guardado.", code = 200 });
            }
            return new JsonResult(new { succes = false, message = "El Actor ya se encuentra en la base de datos.", code = 500 });
        }
    }
}
