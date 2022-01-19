using AutoMapper;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;

namespace PeliculasApi.Utils
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();
        }
    }
}
