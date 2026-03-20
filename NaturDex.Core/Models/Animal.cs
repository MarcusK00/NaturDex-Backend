using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturDex.Core.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public string? Species { get; set; }
        public string? ScientificName { get; set; }
        public string? Description { get; set; }
        public string? Region { get; set; }
        public string? DietDetails { get; set; }
        public float AverageWeightKg { get; set; }
        public int AverageLifespan { get; set; }
    }
}
