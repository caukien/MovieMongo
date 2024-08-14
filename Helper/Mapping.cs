using AutoMapper;
using MovieMongo.Dto;
using MovieMongo.Dto.Actor;
using MovieMongo.Dto.Director;
using MovieMongo.Dto.Genre;
using MovieMongo.Dto.Movie;
using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Helper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Genre, CreateGenreDto>().ReverseMap();

            CreateMap<Movie, CreateMovieDto>().ReverseMap();

            CreateMap<Director, CreateDirectorDto>().ReverseMap();

            CreateMap<Actor, CreateActorDto>().ReverseMap();

            //Simple Dto
            CreateMap<Movie, SimpleMovieDto>();
            CreateMap<Genre, SimpleGenreDto>();
        }
    }
}
