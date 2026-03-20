using NaturDex.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturDex.Core.Interfaces
{
    public interface IAnimalRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> UpdateAsync(T entity);
        Task<T> CreateNewAsync(T entity);
        Task DeleteAsync(int id);

    }
}
