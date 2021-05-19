using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventsParser.Implementations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MMAEvents.Infrastructure.Services
{
    public class EventsParserHostedService : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly ILogger<EventsParserHostedService> logger;
        private readonly WikipediaUfcEventsParser parser;
        private readonly HttpClient httpClient;
        private readonly Uri EventsSourceUri;
        private readonly Uri EventsPostUri;
        private readonly TimeSpan IntervalInMinutes;


        public EventsParserHostedService(ILogger<EventsParserHostedService> logger, HttpClient httpClient, EventsParserOptions options)
        {
            this.logger = logger;
            this.parser = new WikipediaUfcEventsParser(httpClient, logger as ILogger<WikipediaUfcEventsParser>);
            this.parser.QuantityOfPastEvents = options.QuantityOfPastEvents;
            this.parser.QuantityOfScheduledEvents = options.QuantityOfScheduledEvents;
            this.EventsSourceUri = options.EventsSourceUri;
            this.EventsPostUri = options.EventsPostUri;
            this.httpClient = httpClient;
            this.IntervalInMinutes = TimeSpan.FromMinutes(options.IntervalInMinutes);
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Events Parser Hosted Service running.");

            timer = new Timer(DoWork, null, TimeSpan.Zero, IntervalInMinutes);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                var events = parser.GetEventsFromWeb(EventsSourceUri).Result;

                var request = new HttpRequestMessage(HttpMethod.Post, EventsPostUri);
                request.Content = new StringContent(JsonSerializer.Serialize(events), Encoding.UTF8, "application/json");
                var response = httpClient.Send(request);
                if (!response.IsSuccessStatusCode)
                    logger.LogError(new EventId(), $"HttpError in EventsParserHostedService. Status code: {response.StatusCode}", response);
            }
            catch (Exception e)
            {
                logger.LogError(new EventId(), e, "Error in EventsParserHostedService");
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Events Parser Hosted Service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}