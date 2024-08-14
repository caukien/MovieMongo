using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Interfaces
{
    public interface IMovieRepository
    {
        Task<ICollection<Movie>> GetMovies();
        Task<Movie> GetMovie(string id);
        Task<bool> Exists(string id);
        Task<bool> NameExists(string name);
        Task Create(Movie movie);
        Task Update(string id, Movie movie);
        Task Delete(string id);

        Task<Genre> GetGenreById(string genreId);
        Task<Movie> GetMovieWithDetails(string id);
        Task<ICollection<Movie>> GetMoviesWithDetails();
    }
}
