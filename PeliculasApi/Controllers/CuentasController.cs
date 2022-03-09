using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PeliculasApi.Entidades.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasApi.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        //se inyecta UserManager al contructor de la clase y se asigna como campo con el fin de poder crear un usuario.
        //se inyecta SigningManager al constructor de la clase y se asigna como campo con el fin de poder trabajar con el login.
        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration,SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }


        [HttpPost("crear")]
        public async Task<ActionResult<RespuestaAutenticacion>> Crear([FromBody] CredencialesUsuario credenciales)
        {
            /*inicializando instancia de IdentityUser. y dandole valores a las propiedades UserName y Email, valores que llegaran por medio de la peticion POST
             * desde el frontEnd
             */


            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };

            //resultado :  guarda la creacion del usuario en la base de datos.
            var resultado = await userManager.CreateAsync(usuario, credenciales.Password);

            if (resultado.Succeeded)
            {
                //si el usuario es creado correctamente se crea el token mediante el metodo ConstruirToken
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login([FromBody] CredencialesUsuario credenciales)
        {
            //resultado: contiene el resultado de si el usuario y contraseña utilizados en el login son correctos.
            var resultado = await signInManager.PasswordSignInAsync(credenciales.Email,credenciales.Password,isPersistent:false,lockoutOnFailure:false);

            if (resultado.Succeeded)
            {
                //si el resultado es correecto se crea un token.
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

            private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credenciales)
        {
            var claims = new List<Claim>()
            {
              new Claim("email",credenciales.Email),
              
            };

            //usuario: contiene el usuario buscado en la base de dato segun el email que tenga.
            var usuario  =  await userManager.FindByEmailAsync(credenciales.Email);  

            //claimsDB: busca en la base de dato las claims del usuario.
            var claimsDB = await userManager.GetClaimsAsync(usuario);   

            //agrega a la lista todos los claims que existan en la base de datos que correspondan al usuario.
            claims.AddRange(claimsDB);

            //llave: se crea un llave a partir de la llavejwt cuyo valor se encuentra en appsettings.
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));

            var creds = new SigningCredentials(llave,SecurityAlgorithms.HmacSha256);

            //expiracion: contiene el tiempo en el que el token sera valido;
            var expiracion = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion,

            };

        }


    }
}
