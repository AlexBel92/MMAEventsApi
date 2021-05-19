using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MMAEvents.Api.Models;
using MMAEvents.Api.Models.Json;

namespace MMAEvents.Api.Services
{
    public class CachedEventsViewModelService : IEventsViewModelService
    {
        private readonly IMemoryCache _cache;
        private readonly EventsViewModelService _service;

        public CachedEventsViewModelService(
            IMemoryCache cache, EventsViewModelService service)
        {
            _cache = cache;
            _service = service;
        }

        public async Task<EventDTO> GetEventDTOByIdOrDefault(long id)
        {
            var cacheKey = $"event-{id}";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(30);
                return await _service.GetEventDTOByIdOrDefault(id);
            });
        }

        public async Task<FightCardDTO> GetEventFightCardDTO(long id, string cardName)
        {
            var cachedEvent = await GetEventDTOByIdOrDefault(id);

            return cachedEvent?.FightCard.FirstOrDefault(fightCard => fightCard.Name.Contains(cardName));
        }

        public async Task<IEnumerable<FightCardDTO>> GetEventFightCardsDTOs(long id)
        {
            var cachedEvent = await GetEventDTOByIdOrDefault(id);
            
            return cachedEvent?.FightCard;
        }

        public async Task<IEnumerable<EventDTO>> GetEventsDTOs(int quantity, string name = null, DateTime date = default)
        {
            var cacheKey = $"Events-{quantity}-{name}-{date.ToShortDateString()}";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(30);
                return await _service.GetEventsDTOs(quantity, name, date);
            });
        }

        public async Task PostEventsAsync(IEnumerable<EventJson> events)
        {
            await _service.PostEventsAsync(events);
        }
    }
}