using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Models
{
    public class Director
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Movie { get; set; } = new List<string>();
        [BsonIgnore]
        public List<Movie> Movies { get; set; }

    }
}
