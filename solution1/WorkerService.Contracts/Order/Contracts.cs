using System;

namespace WorkerService.Contracts.Order
{
    public record CreateOrder(Guid OrderId);

    public record OrderCreated(Guid OrderId);
}