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
            _logger.LogInformation("Creating order");
            await context.Publish(new OrderCreated());
            _logger.LogInformation("Order created");
        }
    }
}