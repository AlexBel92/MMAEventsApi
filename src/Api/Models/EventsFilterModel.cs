using System;
using System.ComponentModel.DataAnnotations;

namespace MMAEvents.Api.Models
{
    public class EventsFilterModel
    {
        [MaxLength(15)]
        public string EventName { get; set; }

        public DateTime Date { get; set; }

        [Range(-10, 10)]    
        public int Quantity { get; set; } = 3;
    }
}