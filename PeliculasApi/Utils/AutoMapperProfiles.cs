using AutoMapper;
using NetTopologySuite.Geometries;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;

namespace PeliculasApi.Utils
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();
            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>();
            //CreateMap<ActorCreacionDTO, Actor>().ForMember(x => x.Foto, options => options.Ignore()); //esta linea saca del mapeo la proiedad Foto

            CreateMap<CineCreacionDTO, Cine>().ForMember(x => x.Ubicacion, x => x.MapFrom(dto =>
             geometryFactory.CreatePoint(new Coordinate(dto.Longitud, dto.Latitud))));

            CreateMap<Cine, CineDTO>()
            .ForMember(x => x.Latitud , dto => dto.MapFrom(campo => campo.Ubicacion.Y))
            .ForMember(x => x.Longitud, dto => dto.MapFrom(campo => campo.Ubicacion.X));
        }

    }
}
