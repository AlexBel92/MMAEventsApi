using AutoMapper;
using MMAEvents.ApplicationCore.Entities;
using MMAEvents.Infrastructure.Services.gRPC.Client;

namespace MMAEvents.Infrastructure.Services
{
    public class MappingEventProfile : Profile
    {
        public MappingEventProfile()
        {
            CreateMap<Event, EventData>()
                       .ForMember(
                           eventData => eventData.IsCanceled,
                           options => options.MapFrom(src => src.IsDeleted))
                       .ForMember(
                           eventData => eventData.Date,
                           options => options.MapFrom(src => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(src.Date)))
                        .ForMember(
                           eventData => eventData.FightCards,
                           options => options.MapFrom(src =>
                            src.FightCards.Values));

            CreateMap<FightCard, FightCardData>();
            CreateMap<FightRecord, FightRecordData>();
        }
    }
}