using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMAEvents.Api.Models;
using MMAEvents.Api.Models.Json;

namespace MMAEvents.Api.Services
{
    public interface IEventsViewModelService
    {
        Task PostEventsAsync(IEnumerable<EventJson> events);

        Task<EventDTO> GetEventDTOByIdOrDefault(long id);
        Task<IEnumerable<EventDTO>> GetEventsDTOs(int quantity, string name = default, DateTime date = default);

        Task<IEnumerable<FightCardDTO>> GetEventFightCardsDTOs(long id);

        Task<FightCardDTO> GetEventFightCardDTO(long id, string cardName);
    }
}