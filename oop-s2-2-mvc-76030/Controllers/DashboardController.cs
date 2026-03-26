using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodSafety.mvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodSafety.mvc.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string town, string risk)
        {
            var now = DateTime.Now;

            var inspections = _context.Inspections
                .Include(i => i.Premises)
                .AsQueryable();

            
            if (!string.IsNullOrEmpty(town))
            {
                inspections = inspections.Where(i => i.Premises.Town == town);
            }

            
            if (!string.IsNullOrEmpty(risk))
            {
                inspections = inspections.Where(i => i.Premises.RiskRating == risk);
            }

            
            var inspectionsThisMonth = await inspections.CountAsync(i =>
                i.InspectionDate.Month == now.Month &&
                i.InspectionDate.Year == now.Year);

            
            var failedThisMonth = await inspections.CountAsync(i =>
                i.Outcome == "Fail" &&
                i.InspectionDate.Month == now.Month &&
                i.InspectionDate.Year == now.Year);

            
            var overdueFollowUps = await _context.FollowUps.CountAsync(f =>
                f.Status == "Open" &&
                f.DueDate < now);

            
            ViewBag.InspectionsThisMonth = inspectionsThisMonth;
            ViewBag.FailedThisMonth = failedThisMonth;
            ViewBag.OverdueFollowUps = overdueFollowUps;

            
            ViewBag.Towns = await _context.Premises
                .Select(p => p.Town)
                .Distinct()
                .ToListAsync();

            ViewBag.Risks = new[] { "Low", "Medium", "High" };

            _logger.LogInformation("Dashboard viewed with filters Town={Town}, Risk={Risk}", town, risk);

            return View();
        }
    }
}