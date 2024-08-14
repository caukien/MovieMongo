using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieMongo.Data;
using MovieMongo.Interfaces;
using MovieMongo.Models;

namespace MovieMongo.Repository
{
    public class DirectorRepository : IDirectorRepository
    {
        private readonly IOptions<DbSetting> _dbSetting;
        private readonly IMongoCollection<Director> _directorCollection;
        private readonly IMongoCollection<Movie> _movieCollection;

        public DirectorRepository(IOptions<DbSetting> dbSetting)
        {
            _dbSetting = dbSetting;
            var client = new MongoClient(dbSetting.Value.ConnectionString);
            var data = client.GetDatabase(dbSetting.Value.DatabaseName);
            _directorCollection = data.GetCollection<Director>(dbSetting.Value.DirectorsCollectionName);
            _movieCollection = data.GetCollection<Movie>(dbSetting.Value.MoviesCollectionName);
        }

        public async Task CreateDirector(Director director)
        {
            await _directorCollection.InsertOneAsync(director);
        }

        public async Task DeleteDirector(string id)
        {
            await _directorCollection.DeleteOneAsync(d => d.Id == id);
        }

        public async Task<bool> DirectorExist(string id)
        {
            var count = await _directorCollection.CountDocumentsAsync(d => d.Id == id);
            return count > 0;
        }

        public async Task<bool> DirectorNameExist(string name)
        {
            var count = await _directorCollection.CountDocumentsAsync(d => d.Name == name);
            return count > 0;
        }

        public async Task<Director> GetDirector(string id)
        {
            return await _directorCollection.Find(g => g.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Director>> GetDirectors()
        {
            return await _directorCollection.Find(_ => true).ToListAsync();
        }

        public async Task<ICollection<Director>> GetMoviesWithDirectors()
        {
            var directors = await _directorCollection.Find(_ => true).ToListAsync();
            foreach (var director in directors)
            {
                if (director.Movie != null && director.Movie.Any())
                {
                    director.Movies = await _movieCollection.Find(m => director.Movie.Contains(m.Id)).ToListAsync();
                }
            }
            return directors;
        }

        public async Task<Director> GetMovieWithDirectors(string id)
        {
            var director = await _directorCollection.Find(g => g.Id == id).FirstOrDefaultAsync();
            if (director != null && director.Movie != null && director.Movie.Any())
            {
                director.Movies = await _movieCollection.Find(m => director.Movie.Contains(m.Id)).ToListAsync();
            }
            return director;
        }

        public async Task UpdateDirector(Director director, string id)
        {
            await _directorCollection.ReplaceOneAsync(g => g.Id == director.Id, director);
        }
    }
}
