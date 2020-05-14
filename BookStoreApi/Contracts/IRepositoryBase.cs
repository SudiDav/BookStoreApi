using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IList<T>> FindAll();
        Task<T> FindById(int id);
        Task<bool> Create(T entity);
        Task<bool> IsExists(int id);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save();
    }
}
