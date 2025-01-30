using ECommerceBackend.Repositories;
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
    public class User
    {
        private readonly IRepository<Models.User> _userRepository;

        public User(IRepository<Models.User> userRepository)
        {
            _userRepository = userRepository;
        }

        [FunctionName("UserLogin")]
        public async Task<IActionResult> UserLogin(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/login")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request to login a user.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<Models.User>(requestBody);
            log.LogInformation($"User logged in: {user?.Email}");
            return new OkObjectResult($"User {user?.Email} logged in successfully!");
        }
    }
}
