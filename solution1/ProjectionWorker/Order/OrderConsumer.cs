using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using WorkerService.Contracts.Order;

namespace ProjectionWorker.Order
{
    public class OrderConsumer : IConsumer<OrderCreated>
    {
        private readonly ILogger<OrderConsumer> _logger;

        public OrderConsumer(ILogger<OrderConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            _logger.LogTrace("Order stored");
            return Task.CompletedTask;
        }
    }
}