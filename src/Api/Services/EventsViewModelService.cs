using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MMAEvents.Api.Controllers;
using MMAEvents.Api.Models;
using MMAEvents.Api.Models.Json;
using MMAEvents.ApplicationCore.Entities;
using MMAEvents.ApplicationCore.Interfaces;
using MMAEvents.ApplicationCore.Services;

namespace MMAEvents.Api.Services
{
    public class EventsViewModelService : IEventsViewModelService
    {
        private readonly IRepository<Event> _repository;
        private readonly IEventsUpdateService _updateService;
        private readonly IAppLogger<EventsController> _logger;
        private readonly IMapper _mapper;

        public EventsViewModelService(IRepository<Event> repository, IEventsUpdateService updateService, IAppLogger<EventsController> logger, IMapper mapper)
        {
            _repository = repository;
            _updateService = updateService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task PostEventsAsync(IEnumerable<EventJson> eventsJson)
        {
            await _updateService.PostEventsAsync(_mapper.Map<IEnumerable<Event>>(eventsJson));
        }

        public async Task<EventDTO> GetEventDTOByIdOrDefault(long id)
        {
            var dbEntry = await Task.Run(() => _repository.AsQueryable().Where(e => e.Id == id).FirstOrDefault());

            if (dbEntry is null)
                return default;

            var eventDto = _mapper.Map<EventDTO>(dbEntry);

            return eventDto;
        }

        public async Task<IEnumerable<EventDTO>> GetEventsDTOs(int quantity, string name = default, DateTime date = default)
        {
            var result = new List<EventDTO>();
            var query = _repository.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(e => e.Name.ToLowerInvariant().Contains(name.ToLowerInvariant()));
            }
            else if (date != default)
            {
                query = query.Where(e => e.Date == CreateUtcDateFrom(date));
            }
            else
            {
                query = query.Where(e => e.IsScheduled == quantity >= 0);
            }

            if (quantity >= 0)
                query = query.OrderBy(e => e.Date);
            else
                query = query.OrderByDescending(e => e.Date);

            if (quantity != 0)
            {
                query = query.Take(Math.Abs(quantity));
            }

            var dbEntries = await Task.Run(() => query.ToList());

            result.AddRange(_mapper.Map<IEnumerable<EventDTO>>(dbEntries));

            return result;
        }

        public async Task<IEnumerable<FightCardDTO>> GetEventFightCardsDTOs(long id)
        {
            var eventDto = await GetEventDTOByIdOrDefault(id);

            return eventDto?.FightCard;
        }

        public async Task<FightCardDTO> GetEventFightCardDTO(long id, string cardName)
        {
            var eventDto = await GetEventDTOByIdOrDefault(id);
            return eventDto?.FightCard.FirstOrDefault(fightCard => fightCard.Name.Contains(cardName));
        }

        private static int DefaultTryParse(string text)
        {
            return int.TryParse(text, out int value) ? value : default;
        }

        private static DateTime CreateUtcDateFrom(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
