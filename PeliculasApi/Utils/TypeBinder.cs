using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PeliculasApi.Utils
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nombrePropiedad = bindingContext.ModelName;
            var valor = bindingContext.ValueProvider.GetValue(nombrePropiedad);

            if(valor == ValueProviderResult.None) //pregunta si es que no hay ningun valor
            {
                return Task.CompletedTask;  
            }

            try
            {
                var valorDeseralizado = JsonConvert.DeserializeObject<T>(valor.FirstValue);//Transforma el Json en objeto .NET;
                bindingContext.Result = ModelBindingResult.Success(valorDeseralizado);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(nombrePropiedad, "El valor dado no es del tipo de dato esperado.");
            }
            return Task.CompletedTask;  
        }
    }
}
