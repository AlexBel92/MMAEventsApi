using MMAEvents.ApplicationCore.Entities;
using MMAEvents.Infrastructure.Data.JsonModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using MongoDB.Driver;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MMAEvents.Infrastructure.Data
{

    public class MongoDbContextSeed
    {
        public static async Task SeedAsync(MongoDbContext context, ILoggerFactory loggerFactory, string path = default, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                if (!context.Set<Event>().AsQueryable().Any())
                {
                    var events = string.IsNullOrWhiteSpace(path) ? GetPreconfiguredEvents() : GetEventsFromFile(path);
                    foreach (var e in events)
                    {
                        e.Id = await context.GetNextSequenceValueFor<Event>();
                    }

                    await context.Set<Event>().InsertManyAsync(events);
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 5)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<MongoDbContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(context, loggerFactory, path, retryForAvailability);
                }
                throw;
            }
        }

        private static IEnumerable<Event> GetPreconfiguredEvents()
        {
            return new List<Event>()
            {
                new Event
                {
                    Name = "UFC 261: Usman vs. Masvidal 2",
                    Date = new DateTime(2021, 04, 24),
                    Venue = "VyStar Veterans Memorial Arena",
                    Location = "Jacksonville, Florida, U.S.",
                    ImgSrc = new Uri(@"//upload.wikimedia.org/wikipedia/en/thumb/b/bd/UFC_on_ESPN_22.jpeg/220px-UFC_on_ESPN_22.jpeg"),
                    FightCards = new Dictionary<string, FightCard>
                    {
                        {
                            "Main Card",
                            new FightCard()
                            {
                                Name = "Main Card",
                                Fights = new List<FightRecord>()
                                {
                                    new FightRecord()
                                    {
                                        WeightClass = "Welterweight",
                                        FirtsFighter = "Kamaru Usman (c)",
                                        SecondFighter = "Jorge Masvidal"
                                    },
                                    new FightRecord()
                                    {
                                        WeightClass = "Women\u2019s Flyweight",
                                        FirtsFighter = "Valentina Shevchenko (c)",
                                        SecondFighter = "J\u00E9ssica Andrade"
                                    }
                                }
                            }
                        },
                        {
                            "Preliminary Card (ESPN / ESPN\u002B)",
                            new FightCard()
                            {
                                Name = "Main Card",
                                Fights = new List<FightRecord>()
                                {
                                    new FightRecord()
                                    {
                                        WeightClass = "Middleweight",
                                        FirtsFighter = "Karl Roberson",
                                        SecondFighter = "Brendan Allen"
                                    }
                                }
                            }
                        }

                    }
                },
                new Event
                {
                    Name = "UFC on ESPN: Whittaker vs. Gastelum",
                    Date = new DateTime(2021, 04, 17),
                    Venue = "UFC Apex",
                    Location = "Las Vegas, Nevada, U.S.",
                    ImgSrc = new Uri(@"//upload.wikimedia.org/wikipedia/en/thumb/b/bd/UFC_on_ESPN_22.jpeg/220px-UFC_on_ESPN_22.jpeg"),
                    FightCards = new Dictionary<string, FightCard>
                    {
                        {
                            "Main card (ESPN / ESPN\u002B)",
                            new FightCard()
                            {
                                Name = "Main card (ESPN / ESPN\u002B)",
                                Fights = new List<FightRecord>()
                                {
                                    new FightRecord()
                                    {
                                        WeightClass = "Middleweight",
                                        FirtsFighter = "Robert Whittaker",
                                        SecondFighter = "Kelvin Gastelum",
                                        Method = "Decision (unanimous) (50\u201345, 50\u201345, 50\u201345)",
                                        Round = "5",
                                        Time = "5:00"
                                    },
                                    new FightRecord()
                                    {
                                        WeightClass = "Heavyweight",
                                        FirtsFighter = "Andrei Arlovski",
                                        SecondFighter = "Chase Sherman",
                                        Method = "Decision (unanimous) (29\u201328, 29\u201328, 29\u201328)",
                                        Round = "3",
                                        Time = "5:00"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static IEnumerable<Event> GetEventsFromFile(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                var json = streamReader.ReadToEnd();
                return JsonSerializer.Deserialize<IEnumerable<EventJson>>(json).Select(e => new Event()
                {
                    IsDeleted = e.IsCancelled,
                    Name = e.EventName,
                    Date = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, 0, 0, 0, DateTimeKind.Utc),
                    ImgSrc = string.IsNullOrWhiteSpace(e.ImgSrc) ? default : new Uri(e.ImgSrc),
                    Venue = e.Venue,
                    Location = e.Location,
                    IsScheduled = e.IsScheduled,
                    FightCards = e.FightCard.ToDictionary(kvp => kvp.Key, kvp =>
                    new FightCard()
                    {
                        Name = kvp.Key,
                        Fights = kvp.Value.Select(fight => new FightRecord()
                        {
                            WeightClass = fight.WeightClass,
                            FirtsFighter = fight.FirtsFighter,
                            SecondFighter = fight.SecondFighter,
                            Method = fight.Method,
                            Round = fight.Round,
                            Time = fight.Time
                        }).ToList()
                    })
                }).ToList();
            }
        }
    }


}