using System;
using System.Net.Http;
using System.Text;
using AutoMapper;
using MMAEvents.ApplicationCore.Entities;
using MMAEvents.ApplicationCore.Interfaces;
using MMAEvents.Infrastructure.Services.gRPC.Client;

namespace MMAEvents.Infrastructure.Services
{   
    public class TelegramRestEventChangeClient : ITelegramEventChangeClient
    {
        private readonly HttpClient httpClient;
        private readonly IMapper mapper;
        private readonly IAppLogger<TelegramRestEventChangeClient> logger;
        private readonly Uri uri;

        public TelegramRestEventChangeClient(HttpClient httpClient, IMapper mapper, IAppLogger<TelegramRestEventChangeClient> logger, Uri uri)
        {
            this.httpClient = httpClient;
            this.mapper = mapper;
            this.logger = logger;
            this.uri = uri;
        }

        public void SendChangedEvents(Event oldEventData, Event newEventData)
        {
            try
            {
                var changes = new Changes();

                changes.OldEventData = mapper.Map<EventData>(oldEventData);
                changes.NewEventData = mapper.Map<EventData>(newEventData);

                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Content = new StringContent(changes.ToJson(), Encoding.UTF8, "application/json");

                var response = httpClient.Send(request);

                if (!response.IsSuccessStatusCode)
                    logger.LogError($"TelegramRestEventChangeClient response was: {response.StatusCode}", response);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
            }
        }
    }
}