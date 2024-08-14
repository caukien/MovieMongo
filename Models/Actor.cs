using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Models
{
    public class Actor
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string Image { get; set; }
        public string PublicId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Movie { get; set; } = new List<string>();
        [BsonIgnore]
        public List<Movie> Movies { get; set; }
    }
}
