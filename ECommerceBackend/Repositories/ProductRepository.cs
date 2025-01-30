using ECommerceBackend.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceBackend.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public ProductRepository(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer("ECommerceDB", "Products");
        }

        public async Task<Product> AddAsync(Product entity)
        {
            return await _container.CreateItemAsync(entity, new PartitionKey(entity.Id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<Product>(id, new PartitionKey(id));
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await Task.FromResult(_container.GetItemLinqQueryable<Product>().ToList());
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            return await _container.ReadItemAsync<Product>(id, new PartitionKey(id));
        }

        public async Task<Product> UpdateAsync(string id, Product entity)
        {
            return await _container.UpsertItemAsync(entity, new PartitionKey(id));
        }
    }
}
