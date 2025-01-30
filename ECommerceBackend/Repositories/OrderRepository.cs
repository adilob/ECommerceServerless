using ECommerceBackend.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceBackend.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public OrderRepository(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer("ECommerceDB", "Orders");
        }

        public async Task<Order> AddAsync(Order entity)
        {
            return await _container.CreateItemAsync(entity, new PartitionKey(entity.UserId));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<Order>(id, new PartitionKey(id));
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await Task.FromResult(_container.GetItemLinqQueryable<Order>().ToList());
        }

        public async Task<Order> GetByIdAsync(string id)
        {
            return await _container.ReadItemAsync<Order>(id, new PartitionKey(id));
        }

        public async Task<Order> UpdateAsync(string id, Order entity)
        {
            return await _container.UpsertItemAsync(entity, new PartitionKey(id));
        }
    }
}
