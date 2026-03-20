using NaturDex.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NaturDex.Core.Models;

using NaturDex.Core.Data;

namespace NaturDex.Core.Repositories
{
    public class AnimalRepository : IRepository<Animal>
    {
        private readonly NaturDexDbContext _dbContext;

        public AnimalRepository(NaturDexDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Animal> CreateAsync(Animal animal)
        {
            if(animal == null) throw new ArgumentNullException(nameof(animal));

            try
            {
                _dbContext.Animals.Add(animal);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex) { throw new InvalidOperationException("Failed to create animal.", ex);}

            return animal;
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be greater than 0", nameof(id));

            Animal animal = await GetByIdAsync(id);

            if (animal == null) throw new InvalidOperationException($"Animal with id {id} not found.");

            try
            {
                _dbContext.Animals.Remove(animal);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex) { throw new InvalidOperationException("Failed to delete animal.", ex);}
        }

        public Task<List<Animal>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Animal> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Animal> UpdateAsync(Animal entity)
        {
            throw new NotImplementedException();
        }
    }
}
