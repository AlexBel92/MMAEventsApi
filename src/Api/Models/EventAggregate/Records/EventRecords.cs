using System;
using System.Collections.Generic;

namespace MMAEvents.Api.Models.Records
{
    public record EventDTO(string Name, DateTime Date, string ImgSrc, string Location, bool IsScheduled, List<FightCardDTO> FightCard);
    public record FightCardDTO(string Name, List<FightDTO> Fights);
    public record FightDTO(string WeightClass, string FirtsFighter, string SecondFighter, string Method, int Round, string Time);
}