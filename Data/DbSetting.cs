using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Data
{
    public class DbSetting
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string DirectorsCollectionName { get; set; }
        public string ActorsCollectionName { get; set; }
        public string GenresCollectionName { get; set; }
        public string MoviesCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
    }
}
