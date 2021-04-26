using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using WorkerService.Contracts.Order;

namespace WorkerService.Order
{
    public class OrderAggregate : IConsumer<CreateOrder>
    {
        private readonly ILogger<OrderAggregate> _logger;
        private readonly WorkerDbContext _dbContext;

        public OrderAggregate(ILogger<OrderAggregate> logger, WorkerDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<CreateOrder> context)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
                {["OrderId"] = context.Message.OrderId});
            _logger.LogInformation("Creating order");
            _dbContext.Orders.Add(new OrderDao() {Id = context.Message.OrderId});
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Order created");
            await context.Publish(new OrderCreated(context.Message.OrderId));
        }
    }
}