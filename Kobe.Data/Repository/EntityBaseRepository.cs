using Kobe.Data.DBMapping;
using Kobe.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kobe.Data.Repository
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _context;
        public EntityBaseRepository(IBookstoreDatabaseSettings settings)
        {
            //var client = new MongoClient(settings.ConnectionString);
            //var database = client.GetDatabase(settings.DatabaseName);

            //_context = database.GetCollection<T>(settings.BooksCollectionName);

            var client = new MongoClient("mongodb://kobe:kobe@192.168.0.5:27017/kobe?strict=false");
            var database = client.GetDatabase("kobe");        
            _context = database.GetCollection<T>("Books");
        }
        public List<T> GetAllNew()
        {
           return _context.Find(book => true).ToList();
        }
    }
}
