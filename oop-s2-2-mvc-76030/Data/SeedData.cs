using FoodSafety.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSafety.mvc.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Premises.Any())
                return; // DB already seeded

            // 🏠 Premises (12)
            var premisesList = new List<Premises>
            {
                new Premises { Name = "Cafe One", Address = "1 Main St", Town = "Dublin", RiskRating = "High" },
                new Premises { Name = "Pizza House", Address = "2 Main St", Town = "Dublin", RiskRating = "Medium" },
                new Premises { Name = "Burger Spot", Address = "3 Main St", Town = "Cork", RiskRating = "Low" },
                new Premises { Name = "Sushi Place", Address = "4 Main St", Town = "Cork", RiskRating = "High" },
                new Premises { Name = "Bakery Bliss", Address = "5 Main St", Town = "Galway", RiskRating = "Low" },
                new Premises { Name = "Deli Fresh", Address = "6 Main St", Town = "Galway", RiskRating = "Medium" },
                new Premises { Name = "Steak House", Address = "7 Main St", Town = "Dublin", RiskRating = "High" },
                new Premises { Name = "Taco Town", Address = "8 Main St", Town = "Cork", RiskRating = "Medium" },
                new Premises { Name = "Healthy Bites", Address = "9 Main St", Town = "Galway", RiskRating = "Low" },
                new Premises { Name = "Seafood Shack", Address = "10 Main St", Town = "Dublin", RiskRating = "High" },
                new Premises { Name = "Grill Master", Address = "11 Main St", Town = "Cork", RiskRating = "Medium" },
                new Premises { Name = "Coffee Corner", Address = "12 Main St", Town = "Galway", RiskRating = "Low" }
            };

            context.Premises.AddRange(premisesList);
            await context.SaveChangesAsync();

            // 🔍 Inspections (25)
            var inspections = new List<Inspection>();
            var rnd = new Random();

            foreach (var p in premisesList)
            {
                for (int i = 0; i < 2; i++)
                {
                    var score = rnd.Next(50, 100);

                    inspections.Add(new Inspection
                    {
                        PremisesId = p.Id,
                        InspectionDate = DateTime.Now.AddDays(-rnd.Next(0, 60)),
                        Score = score,
                        Outcome = score < 70 ? "Fail" : "Pass",
                        Notes = "Auto generated"
                    });
                }
            }

            context.Inspections.AddRange(inspections);
            await context.SaveChangesAsync();

            // 📌 FollowUps (10)
            var followUps = new List<FollowUp>();

            foreach (var i in inspections.Take(10))
            {
                followUps.Add(new FollowUp
                {
                    InspectionId = i.Id,
                    DueDate = DateTime.Now.AddDays(-rnd.Next(1, 10)), // some overdue
                    Status = "Open"
                });
            }

            context.FollowUps.AddRange(followUps);
            await context.SaveChangesAsync();
        }
    }
}