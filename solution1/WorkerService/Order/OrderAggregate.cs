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

        public OrderAggregate(ILogger<OrderAggregate> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateOrder> context)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
                {["OrderId"] = context.Message.OrderId});
            _logger.LogInformation("Creating order");
            await context.Publish(new OrderCreated(context.Message.OrderId));
            _logger.LogInformation("Order created");
        }
    }
}