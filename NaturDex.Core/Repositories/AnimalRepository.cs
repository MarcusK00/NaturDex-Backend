using NaturDex.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NaturDex.Core.Models;

using NaturDex.Core.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Animal>> GetAllAsync()
        {
            try
            {
              return await _dbContext.Animals.ToListAsync();
            } catch (DbUpdateException ex){ throw new InvalidOperationException("Failed to retrieve animals.", ex); }
        }

        public async Task<Animal> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be greater than 0", nameof(id));

            Animal? animal = await _dbContext.Animals.FindAsync(id);

            if (animal == null) throw new InvalidOperationException($"Failed to retrieve animal with id: {id}");

            return animal;
        }

        public async Task<Animal> UpdateAsync(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));

            if (animal.Id <= 0)
                throw new ArgumentException("Animal Id must be greater than 0.", nameof(animal.Id));

            var existingAnimal = await _dbContext.Animals.FindAsync(animal.Id);

            if (existingAnimal == null) throw new InvalidOperationException($"Animal with id {animal.Id} not found.");

            existingAnimal.Spieces = animal.Spieces;
            existingAnimal.ScientificName = animal.ScientificName;
            existingAnimal.Description = animal.Description;
            existingAnimal.Region = animal.Region;
            existingAnimal.DietDetails = animal.DietDetails;
            existingAnimal.AverageWeightKg = animal.AverageWeightKg;
            existingAnimal.AverageLifespan = animal.AverageLifespan;

            try
            {
              await _dbContext.SaveChangesAsync();
            } catch (DbUpdateException ex) { throw new InvalidOperationException("Failed to update animal.", ex); }
            
            return existingAnimal;
        }
    }
}
