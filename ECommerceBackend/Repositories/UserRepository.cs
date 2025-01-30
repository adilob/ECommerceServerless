using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceBackend.Repositories
{
    public class UserRepository : IRepository<Models.User>
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public UserRepository(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer("ECommerceDB", "Users");
        }

        public async Task<Models.User> AddAsync(Models.User entity)
        {
            return await _container.CreateItemAsync(entity, new PartitionKey(entity.Email));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<Models.User>(id, new PartitionKey(id));
        }

        public async Task<IEnumerable<Models.User>> GetAllAsync()
        {
            return await Task.FromResult(_container.GetItemLinqQueryable<Models.User>().ToList());
        }

        public async Task<Models.User> GetByIdAsync(string id)
        {
            return await _container.ReadItemAsync<Models.User>(id, new PartitionKey(id));
        }

        public async Task<Models.User> UpdateAsync(string id, Models.User entity)
        {
            return await _container.UpsertItemAsync(entity, new PartitionKey(id));
        }
    }
}
