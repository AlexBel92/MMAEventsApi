using MMAEvents.ApplicationCore.Entities;

namespace MMAEvents.Infrastructure.Services
{
    public interface ITelegramEventChangeClient
    {
        void SendChangedEvents(Event oldEventData, Event newEventData);
    }
}