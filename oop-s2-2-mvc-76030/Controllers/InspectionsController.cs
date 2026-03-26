using FoodSafety.Domain.Models;
using FoodSafety.mvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace FoodSafety.mvc.Controllers
{
    [Authorize(Roles = "Admin,Inspector")]
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InspectionsController> _logger;

        public InspectionsController(ApplicationDbContext context, ILogger<InspectionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ LIST
        public async Task<IActionResult> Index()
        {
            var inspections = _context.Inspections
                .Include(i => i.Premises);

            return View(await inspections.ToListAsync());
        }

        // ✅ DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var inspection = await _context.Inspections
                .Include(i => i.Premises)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inspection == null)
                return NotFound();

            return View(inspection);
        }

        // ✅ GET CREATE
        public IActionResult Create()
        {
            ViewData["PremisesId"] = new SelectList(_context.Premises, "Id", "Name");
            return View();
        }

        // ✅ POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inspection inspection)
        {
            if (inspection.Score < 0 || inspection.Score > 100)
            {
                ModelState.AddModelError("Score", "Score must be between 0 and 100");
            }

            if (ModelState.IsValid)
            {
                // Business rule
                inspection.Outcome = inspection.Score < 70 ? "Fail" : "Pass";

                _context.Add(inspection);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Inspection created: PremisesId={PremisesId}, Score={Score}, Outcome={Outcome}",
                    inspection.PremisesId, inspection.Score, inspection.Outcome);

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid inspection creation attempt");

            ViewData["PremisesId"] = new SelectList(_context.Premises, "Id", "Name", inspection.PremisesId);
            return View(inspection);
        }

        // ✅ GET EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null)
                return NotFound();

            ViewData["PremisesId"] = new SelectList(_context.Premises, "Id", "Name", inspection.PremisesId);
            return View(inspection);
        }

        // ✅ POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inspection inspection)
        {
            if (id != inspection.Id)
                return NotFound();

            if (inspection.Score < 0 || inspection.Score > 100)
            {
                ModelState.AddModelError("Score", "Score must be between 0 and 100");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    inspection.Outcome = inspection.Score < 70 ? "Fail" : "Pass";

                    _context.Update(inspection);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Inspection updated: {Id}", inspection.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Inspections.Any(e => e.Id == inspection.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Invalid inspection update attempt");

            ViewData["PremisesId"] = new SelectList(_context.Premises, "Id", "Name", inspection.PremisesId);
            return View(inspection);
        }

        // ✅ GET DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var inspection = await _context.Inspections
                .Include(i => i.Premises)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inspection == null)
                return NotFound();

            return View(inspection);
        }

        // ✅ POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);

            if (inspection != null)
            {
                _context.Inspections.Remove(inspection);
                await _context.SaveChangesAsync();

                _logger.LogWarning("Inspection deleted: {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}