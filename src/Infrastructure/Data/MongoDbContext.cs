using MongoDB.Driver;
using System.Threading.Tasks;
using System.Threading;
using MMAEvents.ApplicationCore.Entities;
using MongoDB.Bson;
using System.Collections.Generic;

namespace MMAEvents.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;

        public MongoDbContext(IMongoDbContextSettings settings)
        {
            var mongoClient = new MongoClient(settings.ConnectionString);
            _database = mongoClient.GetDatabase(settings.DatabaseName);
            _collectionName = settings.CollectionName;

            CreateIndexes(_database.GetCollection<BsonDocument>(_collectionName));
        }

        public IMongoCollection<T> Set<T>() where T : BaseEntity => _database.GetCollection<T>(_collectionName);
        
        public async Task<long> GetNextSequenceValueFor<T>(CancellationToken cancellationToken = default) where T : BaseEntity
        {
            var sequenceName = "sequence_" + typeof(T).BaseType;

            var value = await Sequence.GetNextSequenceValueAsync(sequenceName, _database);

            return value;
        }

        private void CreateIndexes(IMongoCollection<BsonDocument> mongoCollection)
        {
            var indexes = new List<CreateIndexModel<BsonDocument>>()
            {
                new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("Date")),
                new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("IsScheduled")),
                new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Ascending("IsDeleted"))
            };

            _database.GetCollection<BsonDocument>(_collectionName).Indexes.CreateMany(indexes);
        }
    }
}