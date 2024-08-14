using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Interfaces
{
    public interface IDirectorRepository
    {
        Task<ICollection<Director>> GetDirectors();
        Task<Director> GetDirector(string id);
        Task<bool> DirectorExist(string id);
        Task<bool> DirectorNameExist(string name);
        Task CreateDirector(Director director);
        Task UpdateDirector(Director director, string id);
        Task DeleteDirector(string id);

        Task<Director> GetMovieWithDirectors(string id);
        Task<ICollection<Director>> GetMoviesWithDirectors();
    }
}
