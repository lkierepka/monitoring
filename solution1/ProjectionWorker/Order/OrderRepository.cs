using System.Threading.Tasks;
using MongoDB.Driver;
using ProjectionWorker.Config;

namespace ProjectionWorker.Order
{
    public interface IOrderRepository
    {
        Task Insert(Order order);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _collection;

        public OrderRepository(IMongoConfiguration mongoConfiguration)
        {
            var client = new MongoClient(mongoConfiguration.ConnectionString);
            var database = client.GetDatabase(mongoConfiguration.Database);
            _collection = database.GetCollection<Order>(mongoConfiguration.OrderCollection);
        }

        public async Task Insert(Order order)
        {
            await _collection.InsertOneAsync(order);
        }
    }
}