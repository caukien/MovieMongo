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
    public class ActorRepository : IActorRepository
    {
        private readonly IOptions<DbSetting> _dbSetting;
        private IMongoCollection<Actor> _actor;
        public ActorRepository(IOptions<DbSetting> dbSetting)
        {
            _dbSetting = dbSetting;
            var client = new MongoClient(_dbSetting.Value.ConnectionString);
            var db = client.GetDatabase(_dbSetting.Value.DatabaseName);
            _actor = db.GetCollection<Actor>(_dbSetting.Value.ActorsCollectionName);
        }

        public async Task Create(Actor actor)
        {
            await _actor.InsertOneAsync(actor);
        }

        public async Task Delete(string id)
        {
            await _actor.DeleteOneAsync(a => a.Id == id);
        }

        public async Task<bool> Exists(string id)
        {
            var count = await _actor.CountDocumentsAsync(a => a.Id == id);
            return count > 0;
        }

        public async Task<Actor> GetActor(string id)
        {
            return await _actor.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ICollection<Actor>> GetActors()
        {
            return await _actor.Find(_ => true).ToListAsync();
        }

        public async Task<bool> NameExists(string name)
        {
            var nameTrimed = name.Trim().ToLower();
            var count = await _actor.CountDocumentsAsync(a => a.Name.Trim().ToLower() == nameTrimed);
            return count > 0;
        }

        public async Task Update(string id, Actor actor)
        {
            await _actor.ReplaceOneAsync(a => a.Id == id, actor);
        }
    }
}
