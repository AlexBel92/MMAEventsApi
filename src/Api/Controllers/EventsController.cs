using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MMAEvents.Api.Models;
using MMAEvents.Api.Filters;
using MMAEvents.Api.Services;
using Swashbuckle.AspNetCore.Annotations;
using MMAEvents.Api.Models.Json;

namespace MMAEvents.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventsViewModelService eventsService;

        public EventsController(IEventsViewModelService eventsService)
        {
            this.eventsService = eventsService;
        }

        [HttpGet("")]
        [ValidateModel]
        [SwaggerOperation(
            Summary = "Gets specified events",
            Description = "Gets specified events",
            OperationId = "events.get",
            Tags = new[] { "EventsEndpoints" })
        ]
        [SwaggerResponse(200, "The events was found", typeof(IEnumerable<EventDTO>))]
        [SwaggerResponse(404, "The events not found")]
        [SwaggerResponse(400, "Invalid data for filtering")]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEvents([FromQuery] EventsFilterModel filter)
        {
            var eventDTOs = await eventsService.GetEventsDTOs(filter.Quantity, filter.EventName, filter.Date);

            if (eventDTOs is null || !eventDTOs.Any())
            {
                return NotFound();
            }

            return eventDTOs.ToList();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Gets event by id",
            Description = "Gets event by id",
            OperationId = "event.get",
            Tags = new[] { "EventsEndpoints" })
        ]
        [SwaggerResponse(200, "The event was found", typeof(EventDTO))]
        [SwaggerResponse(404, "The event not found")]
        [ResponseCache(CacheProfileName = "Default30")]
        public async Task<ActionResult<EventDTO>> GetEvent([FromRoute][Range(1, long.MaxValue)] long id)
        {
            var eventDTO = await eventsService.GetEventDTOByIdOrDefault(id);

            if (eventDTO is null)
                return NotFound();

            return eventDTO;
        }

        [HttpGet("{id}/FightCards")]
        [SwaggerOperation(
            Summary = "Gets fight cards of specific event",
            Description = "Gets fight cards of specific event",
            OperationId = "event-fightcards.get",
            Tags = new[] { "FightCardsEndpoints" })
        ]
        [SwaggerResponse(200, "The fight cards was found", typeof(IEnumerable<FightCardDTO>))]
        [SwaggerResponse(404, "The fight cards not found")]
        public async Task<ActionResult<IEnumerable<FightCardDTO>>> GetEventFightCards([FromRoute][Range(1, long.MaxValue)] long id)
        {
            var result = await eventsService.GetEventFightCardsDTOs(id);

            if (result is null || !result.Any())
                return NotFound();

            return result.ToList();
        }

        [HttpGet("{id}/FightCards/{cardName}")]
        [SwaggerOperation(
            Summary = "Gets fight card of specific event by name",
            Description = "Gets fight card of specific event by name",
            OperationId = "event-fightcard.get",
            Tags = new[] { "FightCardsEndpoints" })
        ]
        [SwaggerResponse(200, "The fight card was found", typeof(FightCardDTO))]
        [SwaggerResponse(404, "The fight card not found")]
        public async Task<ActionResult<FightCardDTO>> GetEventFightCard([FromRoute][Range(1, long.MaxValue)] long id, [FromRoute][StringLength(15)] string cardName)
        {
            var cardDTO = await eventsService.GetEventFightCardDTO(id, cardName);

            if (cardDTO is null)
                return NotFound();

            return cardDTO;
        }

        [HttpPost("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ValidateModel]
        public async Task<ActionResult> PostEvents([FromBody] IEnumerable<EventJson> events)
        {
            await eventsService.PostEventsAsync(events);

            return Ok();
        }
    }
}
