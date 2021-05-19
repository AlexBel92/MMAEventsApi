using System;
using AutoMapper;
using Grpc.Net.Client;
using MMAEvents.ApplicationCore.Entities;
using MMAEvents.ApplicationCore.Interfaces;
using MMAEvents.Infrastructure.Services.gRPC.Client;

namespace MMAEvents.Infrastructure.Services
{
    public class TelegramGrpcEventChangeClient : ITelegramEventChangeClient
    {
        private readonly IMapper mapper;
        private readonly IAppLogger<TelegramGrpcEventChangeClient> logger;
        private readonly GrpcChannel gRpcChannel;

        public TelegramGrpcEventChangeClient(IMapper mapper, IAppLogger<TelegramGrpcEventChangeClient> logger, Uri uri)
        {
            this.gRpcChannel = GrpcChannel.ForAddress(uri);
            this.mapper = mapper;
            this.logger = logger;
        }

        public void SendChangedEvents(Event oldEventData, Event newEventData)
        {
            try
            {
                var changes = new Changes();

                changes.OldEventData = mapper.Map<EventData>(oldEventData);
                changes.NewEventData = mapper.Map<EventData>(newEventData);

                var client = new EventChanges.EventChangesClient(gRpcChannel);
                var reply = client.EventChange(changes);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
            }
        }
    }
}