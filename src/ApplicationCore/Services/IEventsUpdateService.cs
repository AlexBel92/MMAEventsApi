using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.ApplicationCore.Entities;

namespace MMAEvents.ApplicationCore.Services
{
    public interface IEventsUpdateService
    {
        event Action<Event, Event> AfterUpdate;
        Task<int> PostEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default);
    }
}