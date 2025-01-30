// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ECommerceBackend.Functions
{
    public static class OrderEvents
    {
        [FunctionName("HandleOrderCreated")]
        public static void HandleOrderCreated(
            [EventGridTrigger] JObject eventGridEvent,
            ILogger log)
        {
            log.LogInformation($"Received event: {eventGridEvent}");

            var eventType = eventGridEvent["eventType"].ToString();
            var orderData = eventGridEvent["data"];

            if (eventType == "OrderCreated")
            {
                log.LogInformation($"Order created: {orderData}");
            }
        }
    }
}
