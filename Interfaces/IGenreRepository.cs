using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Interfaces
{
    public interface IGenreRepository
    {
        Task<ICollection<Genre>> GetGenres();
        Task<Genre> GetGenre(string id);
        Task<bool> Exists(string id);
        Task<bool> NameExists(string name);
        Task CreateGenre(Genre genre);
        Task UpdateGenre(string id, Genre genre);
        Task DeleteGenre(string id);

        Task<Genre> GetGenreWithMovies(string id);
        Task<ICollection<Genre>> GetGenresWithMovies();
    }
}
