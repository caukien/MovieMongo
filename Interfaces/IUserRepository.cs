using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsers();
        Task<User> GetUser(string id);
        Task<bool> Exists(string id);
        Task<bool> NameExists(string name);
        Task Create(User user);
        Task Update(string id, User user);
        Task Delete(string id);
        Task<User> GetuserandPassword(string username, string password);
    }
}
