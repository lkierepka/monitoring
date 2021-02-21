using System.Threading.Tasks;
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

        public OrderController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder()
        {
            await _publishEndpoint.Publish(new CreateOrder());
            return Ok();
        }
    }
}