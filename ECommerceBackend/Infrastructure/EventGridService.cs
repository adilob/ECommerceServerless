using Azure.Messaging.EventGrid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceBackend.Infrastructure
{
    public class EventGridService : IEventGridService
    {
        private readonly EventGridPublisherClient _eventGridPublisherClient;

        public EventGridService(EventGridPublisherClient eventGridPublisherClient)
        {
            _eventGridPublisherClient = eventGridPublisherClient;
        }

        public async Task PublishEventAsync(string subject, string eventType, object data)
        {
            var events = new List<EventGridEvent>
            {
                new EventGridEvent(
                    subject: subject,
                    eventType: eventType,
                    dataVersion: "1.0",
                    data: data)
            };

            await _eventGridPublisherClient.SendEventsAsync(events);
        }
    }
}
