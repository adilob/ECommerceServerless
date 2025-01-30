using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceBackend.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
    }
}
