using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieMongo.Data;
using MovieMongo.Models;
using MovieMongo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Repository
{
    public class GenreRepository : IGenreRepository
    {
        public readonly IMongoCollection<Genre> _genreRepository;
        private readonly IOptions<DbSetting> setting;
        private readonly IMongoCollection<Movie> _movieRepository;

        public GenreRepository(IOptions<DbSetting> setting)
        {
            this.setting = setting;
            var cl = new MongoClient(setting.Value.ConnectionString);
            var db = cl.GetDatabase(setting.Value.DatabaseName);
            _genreRepository = db.GetCollection<Genre>(setting.Value.GenresCollectionName);
            _movieRepository = db.GetCollection<Movie>(setting.Value.MoviesCollectionName);
        }

        public async Task CreateGenre(Genre genre)
        {
            await _genreRepository.InsertOneAsync(genre);
        }

        public async Task DeleteGenre(string id)
        {
            await _genreRepository.DeleteOneAsync(g => g.Id == id);
        }

        public async Task<bool> Exists(string id)
        {
            var count = await _genreRepository.CountDocumentsAsync(g => g.Id == id);
            return count > 0;
        }

        public async Task<Genre> GetGenre(string id)
        {
            return await _genreRepository.Find(g => g.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Genre>> GetGenres()
        {
            return await _genreRepository.Find(_ => true).ToListAsync();
        }

        public async Task<ICollection<Genre>> GetGenresWithMovies()
        {
            var genres = await _genreRepository.Find(_ => true).ToListAsync();
            foreach (var genre in genres)
            {
                if (genre.Movie != null && genre.Movie.Any())
                {
                    genre.Movies = await _movieRepository.Find(m => genre.Movie.Contains(m.Id)).ToListAsync();
                }
            }
            return genres;
        }

        public async Task<Genre> GetGenreWithMovies(string id)
        {
            var genre = await _genreRepository.Find(g => g.Id == id).FirstOrDefaultAsync();
            if (genre != null && genre.Movie != null && genre.Movie.Any())
            {
                genre.Movies = await _movieRepository.Find(m => genre.Movie.Contains(m.Id)).ToListAsync();
            }
            return genre;
        }

        public async Task<bool> NameExists(string name)
        {
            var nametrimed = name.Trim().ToLower();
            var count = await _genreRepository.CountDocumentsAsync(g => g.Name.Trim().ToLower() == nametrimed);
            return count > 0;   
        }

        public async Task UpdateGenre(string id, Genre genre)
        {
            await _genreRepository.ReplaceOneAsync(g => g.Id == id, genre);
        }
    }
}
