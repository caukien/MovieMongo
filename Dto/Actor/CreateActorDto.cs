using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Dto.Actor
{
    public class CreateActorDto
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public IFormFile Image { get; set; }
        //public string PublicId { get; set; }
    }
}
