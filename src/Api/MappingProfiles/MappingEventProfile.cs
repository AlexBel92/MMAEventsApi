using AutoMapper;
using MMAEvents.ApplicationCore.Entities;
using MMAEvents.Api.Models;
using MMAEvents.Api.Models.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MMAEvents.Api.MappingProfiles
{
    public class MappingEventProfile : Profile
    {
        public MappingEventProfile()
        {
            CreateMap<Event, EventDTO>()
                .ForMember(
                    dto => dto.Location,
                    options => options.MapFrom(src => src.Venue + ", " + src.Location))
                .ForMember(
                    dto => dto.ImgSrc,
                    options => options.MapFrom(src => src.ImgSrc.ToString()))
                .ForMember(
                    dto => dto.FightCard,
                    options => options.MapFrom(src => src.FightCards.Values));

            CreateMap<FightCard, FightCardDTO>();

            CreateMap<FightRecord, FightDTO>()
                .ForMember(
                    dto => dto.Round,
                    options => options.MapFrom(src => IntDefaultTryParse(src.Round)));

            CreateMap<EventJson, Event>()
                .ForMember(
                    e => e.Name,
                    options => options.MapFrom(src => src.EventName))
                .ForMember(
                    e => e.ImgSrc,
                    options => options.MapFrom(src => UriDefaultTryParse(src.ImgSrc)))
                .ForMember(
                    e => e.IsDeleted,
                    options => options.MapFrom(src => src.IsCancelled))
                .ForMember(
                    e => e.Date,
                    options => options.MapFrom(src => new DateTime(src.Date.Year, src.Date.Month, src.Date.Day, 0, 0, 0, DateTimeKind.Utc)))
                .ForMember(
                    e => e.FightCards,
                    options => options.MapFrom(src => src.FightCard.Select(
                        kvp => new KeyValuePair<string, FightCard>(
                            kvp.Key,
                            new FightCard()
                            {
                                Name = kvp.Key,
                                Fights = kvp.Value.Select(fight =>
                                new FightRecord()
                                {
                                    WeightClass = fight.WeightClass,
                                    FirtsFighter = fight.FirtsFighter,
                                    SecondFighter = fight.SecondFighter,
                                    Method = fight.Method,
                                    Round = fight.Round,
                                    Time = fight.Time
                                }).ToList()
                            }))));

            CreateMap<FightRecordJson, FightRecord>();
        }

        private static Uri UriDefaultTryParse(string text)
        {
            return Uri.TryCreate(text, UriKind.Absolute, out Uri result) ? result : default;
        }

        private static int IntDefaultTryParse(string text)
        {
            return int.TryParse(text, out int value) ? value : default;
        }
    }
}