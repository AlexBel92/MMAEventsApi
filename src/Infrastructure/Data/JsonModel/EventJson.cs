using System;
using System.Collections.Generic;

namespace MMAEvents.Infrastructure.Data.JsonModel
{
    public class EventJson
    {
        public EventJson()
        {
            IsScheduled = true;
            IsCancelled = false;
        }

        public string EventName { get; set; }
        public DateTime Date { get; set; }
        public string ImgSrc { get; set; }
        public string Venue { get; set; }
        public string Location { get; set; }

        public bool IsScheduled  { get; set; }
        public bool IsCancelled { get; set; }

        public Dictionary<string, List<FightRecordJson>> FightCard { get; set; }
        public List<string> BonusAwards { get; set; }
    }
}