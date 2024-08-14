using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Dto.Movie
{
    public class CreateMovieDto
    {
        public string Title { get; set; }
        public IFormFile Image { get; set; }
        public List<string> GenreIds { get; set; }
        public List<string> DirectorIds { get; set; }
        public List<string> ActorIds { get; set; }
    }
}
