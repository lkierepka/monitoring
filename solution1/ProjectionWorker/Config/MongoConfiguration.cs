namespace ProjectionWorker.Config
{
    public interface IMongoConfiguration
    {
        string? ConnectionString { get; set; }
        string? Database { get; set; }
        string? OrderCollection { get; set; }
    }

    public class MongoConfiguration : IMongoConfiguration
    {
        public string? ConnectionString { get; set; }
        public string? Database { get; set; }
        public string? OrderCollection { get; set; }
    }
}