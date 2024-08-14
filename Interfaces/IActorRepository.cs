using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Interfaces
{
    public interface IActorRepository
    {
        Task<ICollection<Actor>> GetActors();
        Task<Actor> GetActor(string id);
        Task<bool> Exists(string id);
        Task<bool> NameExists(string name);
        Task Create(Actor actor);
        Task Update(string id, Actor actor);
        Task Delete(string id);
    }
}
