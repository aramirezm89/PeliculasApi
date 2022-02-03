using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasApi.APiBehavior;
using PeliculasApi.Filtros;
using PeliculasApi.Utils;

namespace PeliculasApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //implementacion de AUTOMAPPER
            services.AddAutoMapper(typeof(Startup));

            //agregar GeometryFactory a AUTOMAPPER
            services.AddSingleton(provider =>
          
                new MapperConfiguration(config =>
                {
                    var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                    config.AddProfile(new AutoMapperProfiles(geometryFactory));
                }).CreateMapper()
            );

            /*
             * implementacion del servicio para la ejecucion de guardar borrar o editar archivos en AZURE
             * 
             * si queremos guardar los archivos solo de forma local usamos la clase AlmacenadorArchivosLocal
             */

            services.AddTransient<IAlmacenadorArchivos, AlmacenadorAzureStorage>();

            services.AddHttpContextAccessor();

            /*
             * implementacion de la Class ApplicationDbContext y su conexion a bse de datos por medio
             * de la defaultConnection que se encuentra declarada en appsettings.json
             * 
             * UseNetTopologySuite libreria de entityFrameWorkCore que sirve para datos espaciales (longitud,latitud)
             */
            services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("LocalHostConnection"),
            sqlServer => sqlServer.UseNetTopologySuite()));

            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(FiltroDeExcepcion));
                /*
                 * las clases ParsearBadRequest y BehaviorBadRequest implementadas para el tratamiento
                 * de las bad request tipo 400 
                 */
                options.Filters.Add(typeof(ParsearBadRequest));
            }).ConfigureApiBehaviorOptions(BehaviorBadRequest.Parsear);

            /*
             * implementacion de COORS, ademas debe declararse el UseCors en la pipeline.
             */
            services.AddCors(options =>
            {
                //esta variable guarda la URL del frontEnd declarada en el appsettings.json
                var frontEndURL = Configuration.GetValue<string>("frontend_url");

                options.AddDefaultPolicy(builder =>
                {
                builder.WithOrigins(frontEndURL).AllowAnyMethod().AllowAnyHeader()
                    //cabecera que deseo este expuesta en el frontEnd para obtener total de registros de una entidad en BD
                    //definida en clase HttpContextExtensions con el fin de paginacion.
                    .WithExposedHeaders(new string[]{ "cantidadTotalRegistros"});
                });
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PeliculasApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {





            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PeliculasApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();   
            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
