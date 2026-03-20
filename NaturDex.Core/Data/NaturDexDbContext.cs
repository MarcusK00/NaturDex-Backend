using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NaturDex.Core.Models;


namespace NaturDex.Core.Data
{
    public class NaturDexDbContext : DbContext
    {
        public DbSet<Animal> Animals { get; set; }

        public NaturDexDbContext(DbContextOptions<NaturDexDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
