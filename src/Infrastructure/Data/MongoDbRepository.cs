using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.ApplicationCore.Entities;
using MMAEvents.ApplicationCore.Interfaces;
using MongoDB.Driver;

namespace MMAEvents.Infrastructure.Data
{
    public class MongoDbRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly MongoDbContext _context;

        public MongoDbRepository(MongoDbContext context)
        {
            this._context = context;
        }

        public IQueryable<T> AsQueryable()
        {
            return _context.Set<T>().AsQueryable().Where(i => i.IsDeleted == false);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.Id = await _context.GetNextSequenceValueFor<T>();
            await _context.Set<T>().InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task DeleteAsync(T entity, bool hardDelete = false, CancellationToken cancellationToken = default)
        {
            if (hardDelete)
                await HardDeleteAsync(entity, cancellationToken);
            else
                await SoftDeleteAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq(i => i.Id, entity.Id);
            await _context.Set<T>().ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        }

        private async Task HardDeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq(i => i.Id, entity.Id);
            await _context.Set<T>().FindOneAndDeleteAsync(filter, cancellationToken: cancellationToken);
        }
        private async Task SoftDeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq(i => i.Id, entity.Id);
            var update = Builders<T>.Update.Set(i => i.IsDeleted, true);
            await _context.Set<T>().FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}