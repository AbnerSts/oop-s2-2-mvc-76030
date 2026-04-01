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
            var inspections = _context.Inspections
                .Include(i => i.Premises)
                .AsQueryable();

            // ✅ FILTER BY TOWN
            if (!string.IsNullOrEmpty(town))
            {
                inspections = inspections.Where(i =>
                    i.Premises.Town.ToLower() == town.ToLower());
            }

            // ✅ FILTER BY RISK
            if (!string.IsNullOrEmpty(risk))
            {
                inspections = inspections.Where(i =>
                    i.Premises.RiskRating.ToLower() == risk.ToLower());
            }

            // ✅ TOTAL INSPECTIONS
            var inspectionsCount = await inspections.CountAsync();

            // ✅ FAILED INSPECTIONS
            var failedCount = await inspections.CountAsync(i =>
                i.Outcome == "Fail");

            // ✅ OVERDUE FOLLOW-UPS (FILTERED TOO)
            var followUpsQuery = _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .Where(f => f.Status == "Open" && f.DueDate < DateTime.Now)
                .AsQueryable();

            if (!string.IsNullOrEmpty(town))
            {
                followUpsQuery = followUpsQuery.Where(f =>
                    f.Inspection.Premises.Town.ToLower() == town.ToLower());
            }

            if (!string.IsNullOrEmpty(risk))
            {
                followUpsQuery = followUpsQuery.Where(f =>
                    f.Inspection.Premises.RiskRating.ToLower() == risk.ToLower());
            }

            var overdueFollowUps = await followUpsQuery.CountAsync();

            // ✅ SEND DATA TO VIEW
            ViewBag.InspectionsThisMonth = inspectionsCount;
            ViewBag.FailedThisMonth = failedCount;
            ViewBag.OverdueFollowUps = overdueFollowUps;

            // ✅ DROPDOWNS
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