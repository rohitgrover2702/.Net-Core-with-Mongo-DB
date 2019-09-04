using Kobe.Data.DBMapping;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kobe.Data.Context
{
    public class KobeContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IOptions<DBSettings> _settings;


        public KobeContext(IOptions<DBSettings> settings)
        {
            _settings = settings;
            _client = new MongoClient(_settings.Value.ConnectionString);
            _database = _client.GetDatabase(_settings.Value.DatabaseName);
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return _database.GetCollection<TEntity>(typeof(TEntity).Name);
        }
        public IMongoCollection<TEntity> GetCollection<TEntity>(string CollectionName)
        {
            return _database.GetCollection<TEntity>(CollectionName);
        }
    }
}
