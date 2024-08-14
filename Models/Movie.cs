using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Models
{
    public class Movie
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Image { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> GenreIds { get; set; } = new List<string>();
        [BsonIgnore]
        public List<Genre> Genres { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> DirectorIds { get; set; } = new List<string>();
        [BsonIgnore]
        public List<Director> Directors { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> ActorIds { get; set; } = new List<string>();
        [BsonIgnore]
        public List<Actor> Actors { get; set; }
        public string PublicId { get; set; }
    }
}
