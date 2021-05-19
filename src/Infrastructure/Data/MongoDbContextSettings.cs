namespace MMAEvents.Infrastructure.Data
{
    public class MongoDbContextSettings : IMongoDbContextSettings
    {
        public const string Position = "MongoDbContextSettings";

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }

    public interface IMongoDbContextSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}