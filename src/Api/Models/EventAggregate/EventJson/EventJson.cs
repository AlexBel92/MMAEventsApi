using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MMAEvents.Api.Models.Json
{
    public class EventJson
    {
        public EventJson()
        {
        }

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string EventName { get; set; }
        [Required]
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