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
    public class UserRepository : IUserRepository
    {
        private readonly IOptions<DbSetting> _options;
        private readonly IMongoCollection<User> _user;

        public UserRepository(IOptions<DbSetting> options)
        {
            _options = options;
            var cl = new MongoClient(options.Value.ConnectionString);
            var db = cl.GetDatabase(options.Value.DatabaseName);
            _user = db.GetCollection<User>(options.Value.UsersCollectionName);
        }

        public async Task Create(User user)
        {
            await _user.InsertOneAsync(user);
        }

        public async Task Delete(string id)
        {
            await _user.DeleteOneAsync(u => u.Id == id);
        }

        public async Task<bool> Exists(string id)
        {
            var ex = await _user.CountDocumentsAsync(u => u.Id == id);
            return ex > 0;
        }

        public Task<User> GetUser(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<User>> GetUsers()
        {
            return await _user.Find(_ => true).ToListAsync();
        }

        public async Task<User> GetuserandPassword(string username, string password)
        {
            return await _user.Find(u => u.Username == username && u.Password == password).FirstOrDefaultAsync();
        }

        public async Task<bool> NameExists(string name)
        {
            var ex = await _user.CountDocumentsAsync(u => u.Username == name);
            return ex > 0;
        }

        public Task Update(string id, User user)
        {
            throw new NotImplementedException();
        }
    }
}
