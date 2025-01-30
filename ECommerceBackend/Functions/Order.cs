using ECommerceBackend.Infrastructure;
using ECommerceBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace ECommerceBackend.Functions
{
    public class Order
    {
        private readonly IRepository<Models.Order> _orderRepository;
        private readonly IEventGridService _eventGridService;

        public Order(IRepository<Models.Order> orderRepository, IEventGridService eventGridService)
        {
            _orderRepository = orderRepository;
            _eventGridService = eventGridService;
        }

        [FunctionName("AddOrder")]
        //[Authorize]
        public async Task<IActionResult> AddOrder(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request to add a new order.");

            var user = req.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Models.Order>(requestBody);

            var dbOrder = await _orderRepository.AddAsync(order);

            if (dbOrder == null)
            {
                log.LogInformation($"Failed to add order: {order?.Id}");
                return new BadRequestResult();
            }

            await _eventGridService.PublishEventAsync($"orders/{dbOrder.Id}", "OrderCreated", dbOrder);

            log.LogInformation($"Order added: {dbOrder?.Id} and published to Event Grid.");
            return new OkObjectResult(dbOrder);
        }

        [FunctionName("GetOrderById")]
        public async Task<IActionResult> GetOrderById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orders/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"Processing request to get order with ID: {id}");
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
            {
                log.LogInformation($"Order with ID: {id} not found.");
                return new NotFoundResult();
            }

            log.LogInformation($"Order retrieved: {order?.Id}");
            return new OkObjectResult(order);
        }

        [FunctionName("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "orders/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"Processing request to update order with ID: {id}");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Models.Order>(requestBody);

            var dbOrder = await _orderRepository.GetByIdAsync(id);

            if (dbOrder == null)
            {
                log.LogInformation($"Order with ID: {id} not found.");
                return new NotFoundResult();
            }

            await _orderRepository.UpdateAsync(id, order);
            log.LogInformation($"Order updated: {order?.Id}");

            return new NoContentResult();
        }
    }
}
