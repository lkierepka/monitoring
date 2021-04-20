using System.Threading.Tasks;
using Common;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using WorkerService.Contracts.Order;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IIdGenerator _idGenerator;

        public OrderController(IPublishEndpoint publishEndpoint, IIdGenerator idGenerator)
        {
            _publishEndpoint = publishEndpoint;
            _idGenerator = idGenerator;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder()
        {
            await _publishEndpoint.Publish(new CreateOrder(_idGenerator.NewId()));
            return Ok();
        }
    }
}