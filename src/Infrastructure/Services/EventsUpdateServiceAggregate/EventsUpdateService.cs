using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MMAEvents.ApplicationCore.Entities;
using MMAEvents.ApplicationCore.Interfaces;
using MMAEvents.ApplicationCore.Services;

namespace MMAEvents.Infrastructure.Services
{
    public class EventsUpdateService : IEventsUpdateService
    {
        private readonly IRepository<Event> repository;
        private readonly IAppLogger<EventsUpdateService> logger;

        public event Action<Event, Event> AfterUpdate;

        public EventsUpdateService(IRepository<Event> repository, IAppLogger<EventsUpdateService> logger, ITelegramEventChangeClient changeClient)
        {
            this.repository = repository;
            this.logger = logger;
            AfterUpdate += (old, posted) =>
            {
                if (old is null && posted is not null)
                    logger.LogInformation($"Было добавлено новое событие: {posted.Name} {posted.Date}");
                else if (old is not null && posted is not null)
                    logger.LogInformation($"Было изменено событие с ID: {old.Id}");
            };

            if (changeClient != null)
                AfterUpdate += changeClient.SendChangedEvents;
        }

        public async Task<int> PostEventsAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default)
        {
            if (events is null)
                throw new ArgumentNullException(nameof(events));

            var numberOfChangedEvents = 0;

            var postedEvents = events.ToList();
            var dbEvents = new List<Event>(postedEvents.Count);

            var minDate = postedEvents.Min(e => e.Date);
            var maxDate = postedEvents.Max(e => e.Date).AddDays(1);

            dbEvents.AddRange(repository.AsQueryable().Where(e => e.Date >= minDate && e.Date <= maxDate).ToList());

            foreach (var postedEvent in postedEvents)
            {
                var dbEvent = dbEvents.FirstOrDefault(e => e.Date == postedEvent.Date && e.Name.ToLowerInvariant() == postedEvent.Name.ToLowerInvariant());
                if (dbEvent is null)
                    dbEvent = dbEvents.FirstOrDefault(e => e.Name.ToLowerInvariant() == postedEvent.Name.ToLowerInvariant());
                if (dbEvent is null)
                    dbEvent = dbEvents.FirstOrDefault(e => e.Date == postedEvent.Date);

                if (dbEvent is null)
                {
                    var addedEvent = await repository.AddAsync(postedEvent);
                    if (addedEvent is not null)
                    {
                        numberOfChangedEvents++;
                        AfterUpdate?.Invoke(dbEvent, addedEvent);
                    }
                }
                else if (!postedEvent.Equals(dbEvent))
                {
                    postedEvent.Id = dbEvent.Id;

                    await repository.UpdateAsync(postedEvent);
                    numberOfChangedEvents++;
                    AfterUpdate?.Invoke(dbEvent, postedEvent);
                }
            }

            return numberOfChangedEvents;
        }
    }
}