using System;
using System.Collections.Generic;

namespace MMAEvents.Api.Models
{
    public class EventDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string ImgSrc { get; set; }
        public string Location { get; set; }

        public bool IsScheduled  { get; set; }

        public List<FightCardDTO> FightCard { get; set; }
        public List<string> BonusAwards { get; set; }
    }
}