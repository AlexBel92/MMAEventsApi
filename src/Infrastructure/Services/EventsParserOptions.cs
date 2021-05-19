using System;

namespace MMAEvents.Infrastructure.Services
{
    public class EventsParserOptions
    {
        public static string Positin => "EventsParserOptions";

        public int QuantityOfScheduledEvents { get; set; }
        public int QuantityOfPastEvents { get; set; }
        public Uri EventsSourceUri { get; set; }
        public Uri EventsPostUri { get; set; }
        public int IntervalInMinutes { get; set; } = 60 * 12;
    }
}