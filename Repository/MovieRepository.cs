using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MovieMongo.Data;
using MovieMongo.Interfaces;
using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IOptions<DbSetting> _dbSetting;
        private readonly IMongoCollection<Movie> _movie;
        private readonly IMongoCollection<Genre> _genre;
        private readonly IMongoCollection<Genre> _genreRepository;
        private readonly IMongoCollection<Director> _directorRepository;
        private readonly IMongoCollection<Actor> _actorRepository;
        public MovieRepository(IOptions<DbSetting> dbSetting)
        {
            _dbSetting = dbSetting;
            var client = new MongoClient(_dbSetting.Value.ConnectionString);
            var db = client.GetDatabase(_dbSetting.Value.DatabaseName);
            _movie = db.GetCollection<Movie>(_dbSetting.Value.MoviesCollectionName);
            _genre = db.GetCollection<Genre>(_dbSetting.Value.GenresCollectionName);
            _genreRepository = db.GetCollection<Genre>(dbSetting.Value.GenresCollectionName);
            _directorRepository = db.GetCollection<Director>(dbSetting.Value.DirectorsCollectionName);
            _actorRepository = db.GetCollection<Actor>(dbSetting.Value.ActorsCollectionName);
        }

        public async Task Create(Movie movie)
        {
            await _movie.InsertOneAsync(movie);
            await UpdateRelatedEntities(movie);
        }

        public async Task Delete(string id)
        {
            //await _movie.DeleteOneAsync(c => c.Id == id);
            var movie = await _movie.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (movie != null)
            {
                await _movie.DeleteOneAsync(m => m.Id == id);
                await RemoveMovieFromRelatedEntities(movie);
            }
        }

        public async Task<bool> Exists(string id)
        {
            var ex = await _movie.CountDocumentsAsync(c => c.Id == id);
            return ex > 0;
        }

        public async Task<ICollection<Movie>> GetMovies()
        {
            return await _movie.Find(_ => true).ToListAsync();
            //var movies = await _movie.Find(_ => true).ToListAsync();
            //foreach (var movie in movies)
            //{
            //    if (movie.GenreIds != null && movie.GenreIds.Any())
            //    {
            //        movie.Genres = await _genreRepository.GetMoviesByIds(movie.GenreIds);
            //    }
            //}
            //return movies;
        }

        public async Task<Movie> GetMovie(string id)
        {
            return await _movie.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> NameExists(string name)
        {
            var ex = await _movie.CountDocumentsAsync(c => c.Title == name);
            return ex > 0;
        }

        public async Task Update(string id, Movie movie)
        {
            //await _movie.ReplaceOneAsync(c => c.Id == id, movie);
            var existingMovie = await _movie.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (existingMovie == null) return;

            await _movie.ReplaceOneAsync(m => m.Id == id, movie);
            await UpdateRelatedEntitiesOnUpdate(existingMovie, movie);
        }

        public async Task AddMovieToGenre(string genreId, string movieId)
        {
            //var filter = Builders<Genre>.Filter.Eq(g => g.Id, genreId);
            //var update = Builders<Genre>.Update.Push(g => g.Movie, movieId);
            //await _genre.UpdateOneAsync(filter, update);
            var movie = await _movie.Find(m => m.Id == movieId).FirstOrDefaultAsync();
            if (movie != null)
            {
                if (movie.GenreIds == null)
                {
                    movie.GenreIds = new List<string>();
                }

                if (!movie.GenreIds.Contains(genreId))
                {
                    movie.GenreIds.Add(genreId);
                    await _movie.ReplaceOneAsync(m => m.Id == movieId, movie);
                }
            }
        }

        public async Task<Genre> GetGenreById(string genreId)
        {
            return await _genre.Find<Genre>(g => g.Id == genreId).FirstOrDefaultAsync();
        }

        public async Task<Movie> GetMovieWithDetails(string id)
        {
            var movie = await _movie.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (movie != null)
            {
                if (movie.GenreIds != null && movie.GenreIds.Any())
                {
                    movie.Genres = await _genreRepository.Find(g => movie.GenreIds.Contains(g.Id)).ToListAsync();
                }
                if (movie.DirectorIds != null && movie.DirectorIds.Any())
                {
                    movie.Directors = await _directorRepository.Find(d => movie.DirectorIds.Contains(d.Id)).ToListAsync();
                }
                if (movie.ActorIds != null && movie.ActorIds.Any())
                {
                    movie.Actors = await _actorRepository.Find(c => movie.ActorIds.Contains(c.Id)).ToListAsync();
                }
            }
            return movie;
        }

        public async Task<ICollection<Movie>> GetMoviesWithDetails()
        {
            var movies = await _movie.Find(_ => true).ToListAsync();
            foreach (var movie in movies)
            {
                if (movie.GenreIds != null && movie.GenreIds.Any())
                {
                    movie.Genres = await _genreRepository.Find(g => movie.GenreIds.Contains(g.Id)).ToListAsync();
                }
                if (movie.DirectorIds != null && movie.DirectorIds.Any())
                {
                    movie.Directors = await _directorRepository.Find(d => movie.DirectorIds.Contains(d.Id)).ToListAsync();
                }
                if (movie.ActorIds != null && movie.ActorIds.Any())
                {
                    movie.Actors = await _actorRepository.Find(c => movie.ActorIds.Contains(c.Id)).ToListAsync();
                }
            }
            return movies;
        }

        private async Task UpdateRelatedEntities(Movie movie)
        {
            if (movie.GenreIds != null && movie.GenreIds.Any())
            {
                var filter = Builders<Genre>.Filter.In(g => g.Id, movie.GenreIds);
                var update = Builders<Genre>.Update.AddToSet(g => g.Movie, movie.Id);
                await _genreRepository.UpdateManyAsync(filter, update);
            }
            if (movie.DirectorIds != null && movie.DirectorIds.Any())
            {
                var filter = Builders<Director>.Filter.In(d => d.Id, movie.DirectorIds);
                var update = Builders<Director>.Update.AddToSet(d => d.Movie, movie.Id);
                await _directorRepository.UpdateManyAsync(filter, update);
            }
            if (movie.ActorIds != null && movie.ActorIds.Any())
            {
                var filter = Builders<Actor>.Filter.In(c => c.Id, movie.ActorIds);
                var update = Builders<Actor>.Update.AddToSet(c => c.Movie, movie.Id);
                await _actorRepository.UpdateManyAsync(filter, update);
            }
        }

        private async Task RemoveMovieFromRelatedEntities(Movie movie)
        {
            if (movie.GenreIds != null && movie.GenreIds.Any())
            {
                var filter = Builders<Genre>.Filter.In(g => g.Id, movie.GenreIds);
                var update = Builders<Genre>.Update.Pull(g => g.Movie, movie.Id);
                await _genreRepository.UpdateManyAsync(filter, update);
            }
            if (movie.DirectorIds != null && movie.DirectorIds.Any())
            {
                var filter = Builders<Director>.Filter.In(d => d.Id, movie.DirectorIds);
                var update = Builders<Director>.Update.Pull(d => d.Movie, movie.Id);
                await _directorRepository.UpdateManyAsync(filter, update);
            }
            if (movie.ActorIds != null && movie.ActorIds.Any())
            {
                var filter = Builders<Actor>.Filter.In(c => c.Id, movie.ActorIds);
                var update = Builders<Actor>.Update.Pull(c => c.Movie, movie.Id);
                await _actorRepository.UpdateManyAsync(filter, update);
            }
        }

        private async Task UpdateRelatedEntitiesOnUpdate(Movie existingMovie, Movie updatedMovie)
        {
            // Remove movie ID from old genres
            var oldGenreFilter = Builders<Genre>.Filter.In(g => g.Id, existingMovie.GenreIds);
            var oldGenreUpdate = Builders<Genre>.Update.Pull(g => g.Movie, existingMovie.Id);
            await _genreRepository.UpdateManyAsync(oldGenreFilter, oldGenreUpdate);

            // Add movie ID to new genres
            var newGenreFilter = Builders<Genre>.Filter.In(g => g.Id, updatedMovie.GenreIds);
            var newGenreUpdate = Builders<Genre>.Update.AddToSet(g => g.Movie, updatedMovie.Id);
            await _genreRepository.UpdateManyAsync(newGenreFilter, newGenreUpdate);

            // Remove movie ID from old directors
            var oldDirectorFilter = Builders<Director>.Filter.In(d => d.Id, existingMovie.DirectorIds);
            var oldDirectorUpdate = Builders<Director>.Update.Pull(d => d.Movie, existingMovie.Id);
            await _directorRepository.UpdateManyAsync(oldDirectorFilter, oldDirectorUpdate);

            // Add movie ID to new directors
            var newDirectorFilter = Builders<Director>.Filter.In(d => d.Id, updatedMovie.DirectorIds);
            var newDirectorUpdate = Builders<Director>.Update.AddToSet(d => d.Movie, updatedMovie.Id);
            await _directorRepository.UpdateManyAsync(newDirectorFilter, newDirectorUpdate);

            // Remove movie ID from old cast
            var oldCastFilter = Builders<Actor>.Filter.In(c => c.Id, existingMovie.ActorIds);
            var oldCastUpdate = Builders<Actor>.Update.Pull(c => c.Movie, existingMovie.Id);
            await _actorRepository.UpdateManyAsync(oldCastFilter, oldCastUpdate);

            // Add movie ID to new cast
            var newCastFilter = Builders<Actor>.Filter.In(c => c.Id, updatedMovie.ActorIds);
            var newCastUpdate = Builders<Actor>.Update.AddToSet(c => c.Movie, updatedMovie.Id);
            await _actorRepository.UpdateManyAsync(newCastFilter, newCastUpdate);

        }
    }
}
