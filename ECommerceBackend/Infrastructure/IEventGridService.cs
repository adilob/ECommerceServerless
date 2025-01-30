using System.Threading.Tasks;

namespace ECommerceBackend.Infrastructure
{
    public interface IEventGridService
    {
        Task PublishEventAsync(string subject, string eventType, object data);
    }
}
