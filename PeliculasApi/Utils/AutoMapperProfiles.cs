using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using PeliculasApi.Entidades;
using PeliculasApi.Entidades.DTOs;
using System.Collections.Generic;

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
            CreateMap<ActorCreacionDTO, Actor>().ForMember(x => x.Foto, options => options.Ignore()); //esta linea saca del mapeo la proiedad Foto


            /*este mapeo permite que los campos longitud y latitud presentes en CineCreacionDTO se transformen en el campo Ubicacion
             * presente en la entidad Cine 
             */
            CreateMap<CineCreacionDTO, Cine>().ForMember(x => x.Ubicacion, x => x.MapFrom(dto =>
             geometryFactory.CreatePoint(new Coordinate(dto.Longitud, dto.Latitud))));

            /*este mapeo permite extraer del  campo Ubicacion Presente en la entidad Cine (Campo Ubicacion tipo POINT)
             * la latitud y longitud con el fin de completar los campos del mismo nombre que se encuentras en el DTO CineDTO
           */
            CreateMap<Cine, CineDTO>()
            .ForMember(x => x.Latitud, dto => dto.MapFrom(campo => campo.Ubicacion.Y))
            .ForMember(x => x.Longitud, dto => dto.MapFrom(campo => campo.Ubicacion.X));


            CreateMap<PeliculaCreacionDTO, Pelicula>().ForMember(x => x.Poster, opciones => opciones.Ignore())
                .ForMember(x => x.PeliculasGenero, opciones => opciones.MapFrom(MapearPeliculasGeneros))
                .ForMember(x => x.PeliculasCine, opciones => opciones.MapFrom(MapearPeliculasCine))
                .ForMember(x => x.PeliculasActores, opciones => opciones.MapFrom(MapearPeliculasActores));

            CreateMap<Pelicula, PeliculaDTO>()
              .ForMember(x => x.Generos, options => options.MapFrom(MapearPeliculasGeneros))
              .ForMember(x => x.Actores, options => options.MapFrom(MapearPeliculasActores))
              .ForMember(x => x.Cines, options => options.MapFrom(MapearPeliculasCines));

            CreateMap<IdentityUser, UsuarioDTO>();
        }

        private List<CineDTO> MapearPeliculasCines(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<CineDTO>();

            if (pelicula.PeliculasCine != null)
            {
                foreach (var peliculasCines in pelicula.PeliculasCine)
                {
                    resultado.Add(new CineDTO()
                    {
                        Id = peliculasCines.CineId,
                        Nombre = peliculasCines.Cine.Nombre,
                        Latitud = peliculasCines.Cine.Ubicacion.Y,
                        Longitud = peliculasCines.Cine.Ubicacion.X
                    });
                }
            }

            return resultado;
        }

        private List<PeliculaActorDTO> MapearPeliculasActores(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<PeliculaActorDTO>();

            if (pelicula.PeliculasActores != null)
            {
                foreach (var actorPeliculas in pelicula.PeliculasActores)
                {
                    resultado.Add(new PeliculaActorDTO()
                    {
                        Id = actorPeliculas.ActorId,
                        Nombre = actorPeliculas.Actor.Nombre,
                        Foto = actorPeliculas.Actor.Foto,
                        Orden = actorPeliculas.Orden,
                        Personaje = actorPeliculas.Personaje
                    });
                }
            }

            return resultado;
        }

        private List<GeneroDTO> MapearPeliculasGeneros(Pelicula pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<GeneroDTO>();

            if (pelicula.PeliculasGenero != null)
            {
                foreach (var genero in pelicula.PeliculasGenero)
                {
                    resultado.Add(new GeneroDTO() { Id = genero.GeneroId, Nombre = genero.Genero.Nombre });
                }
            }

            return resultado;
        }
        private List<PeliculasActores> MapearPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();

            if (peliculaCreacionDTO.Actores == null)
            {
                return resultado;
            }

            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.Id, Personaje = actor.Personaje });
            }

            return resultado;

        }

        private List<PeliculasGenero> MapearPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGenero>();

            if (peliculaCreacionDTO.GenerosIds == null)
            {
                return resultado;
            }

            foreach (var id in peliculaCreacionDTO.GenerosIds)
            {
                resultado.Add(new PeliculasGenero() { GeneroId = id });
            }

            return resultado;
        }

        private List<PeliculasCines> MapearPeliculasCine(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasCines>();

            if (peliculaCreacionDTO.CinesIds == null)
            {
                return resultado;
            }

            foreach (var id in peliculaCreacionDTO.CinesIds)
            {
                resultado.Add(new PeliculasCines() { CineId = id });
            }

            return resultado;
        }

    }
}
