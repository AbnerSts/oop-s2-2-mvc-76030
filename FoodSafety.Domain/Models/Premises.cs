using FoodSafety.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodSafety.Domain.Models
{
    public class Premises
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }

        public string RiskRating { get; set; }

        [NotMapped] // 🔥 ADD THIS LINE
        public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    }
}