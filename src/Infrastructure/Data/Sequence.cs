using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MMAEvents.Infrastructure.Data
{
    public class Sequence
    {
        private const string collectionName = "sequences";

        public Sequence(string name)
        {
            Name = name;
            Value = 1;
        }

        [BsonId]
        public ObjectId _Id { get; set; }

        public string Name { get; set; }

        public long Value { get; set; }

        internal async static Task<long> GetNextSequenceValueAsync(string sequenceName, IMongoDatabase database, CancellationToken cancellationToken = default)
        {
            if (!await IsExistsAsync(sequenceName, database, cancellationToken))
            {
                await InsertAsync(sequenceName, database, cancellationToken);
            }

            var collection = database.GetCollection<Sequence>(collectionName);
            var filter = Builders<Sequence>.Filter.Eq(a => a.Name, sequenceName);
            var update = Builders<Sequence>.Update.Inc(a => a.Value, 1);
            var sequence = await collection.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);

            return sequence.Value;
        }

        private async static Task InsertAsync(string sequenceName, IMongoDatabase database, CancellationToken cancellationToken = default)
        {
            var collection = database.GetCollection<Sequence>(collectionName);
            await collection.InsertOneAsync(new Sequence(sequenceName), cancellationToken: cancellationToken);
        }

        private async static Task<bool> IsExistsAsync(string sequenceName, IMongoDatabase database, CancellationToken cancellationToken = default)
        {
            var collection = database.GetCollection<Sequence>(collectionName);
            var filter = Builders<Sequence>.Filter.Eq(a => a.Name, sequenceName);
            var sequenceCursor = await collection.FindAsync(filter, cancellationToken: cancellationToken);

            return sequenceCursor.Any();
        }
    }
}