using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using WorkerService.Contracts.Order;

namespace ProjectionWorker.Order
{
    public class OrderConsumer : IConsumer<OrderCreated>
    {
        private readonly ILogger<OrderConsumer> _logger;
        private readonly IOrderRepository _orderRepository;

        public OrderConsumer(ILogger<OrderConsumer> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
                {["OrderId"] = context.Message.OrderId});
            await _orderRepository.Insert(new Order(context.Message.OrderId));
            _logger.LogTrace("Order stored");
        }
    }
}