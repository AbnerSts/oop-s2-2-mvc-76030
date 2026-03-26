using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodSafety.Domain.Models
{
    public class Inspection
    {
        public int Id { get; set; }

        public int PremisesId { get; set; }
        public Premises? Premises { get; set; }

        public DateTime InspectionDate { get; set; }

        public int Score { get; set; }

        public string? Outcome { get; set; } // Pass / Fail

        public string? Notes { get; set; }
    }
}