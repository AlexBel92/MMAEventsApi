using MMAEvents.ApplicationCore.Entities;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace MMAEvents.ApplicationCore.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> AsQueryable();
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, bool hardDelete = false, CancellationToken cancellationToken = default);
    }
}