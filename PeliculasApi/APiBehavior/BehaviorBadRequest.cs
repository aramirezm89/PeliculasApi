using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace PeliculasApi.APiBehavior
{
    public static class BehaviorBadRequest
    {
        public static void Parsear(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var respuesta = new List<string>();
                foreach (var key in actionContext.ModelState.Keys)
                {
                    foreach (var error in actionContext.ModelState[key].Errors)
                    {
                        respuesta.Add($"{key}: {error.ErrorMessage}");
                    }
                }

                return new BadRequestObjectResult(respuesta);
            };
        }
    }
}
