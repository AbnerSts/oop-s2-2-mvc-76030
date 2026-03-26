using FoodSafety.Domain.Models;
using FoodSafety.mvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodSafety.mvc.Controllers
{
    [Authorize(Roles = "Admin,Inspector")]
    public class FollowUpsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FollowUpsController> _logger;

        public FollowUpsController(ApplicationDbContext context, ILogger<FollowUpsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ LIST
        public async Task<IActionResult> Index()
        {
            var followUps = _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises);

            return View(await followUps.ToListAsync());
        }

        // ✅ DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var followUp = await _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (followUp == null)
                return NotFound();

            return View(followUp);
        }

        // ✅ GET CREATE
        public IActionResult Create()
        {
            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id");
            return View();
        }

        // ✅ POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FollowUp followUp)
        {
            var inspection = await _context.Inspections.FindAsync(followUp.InspectionId);

            // ❗ BUSINESS RULE: DueDate must be after InspectionDate
            if (inspection != null && followUp.DueDate < inspection.InspectionDate)
            {
                ModelState.AddModelError("DueDate", "Due date cannot be before inspection date");
                _logger.LogWarning("Invalid FollowUp: DueDate before InspectionDate");
            }

            if (ModelState.IsValid)
            {
                followUp.Status = "Open";
                followUp.ClosedDate = null;

                _context.Add(followUp);
                await _context.SaveChangesAsync();

                _logger.LogInformation("FollowUp created: InspectionId={InspectionId}", followUp.InspectionId);

                return RedirectToAction(nameof(Index));
            }

            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        // ✅ GET EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp == null)
                return NotFound();

            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        // ✅ POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FollowUp followUp)
        {
            if (id != followUp.Id)
                return NotFound();

            // ❗ BUSINESS RULE: Closed must have ClosedDate
            if (followUp.Status == "Closed" && followUp.ClosedDate == null)
            {
                ModelState.AddModelError("ClosedDate", "ClosedDate is required when status is Closed");
                _logger.LogWarning("Invalid FollowUp: Closed without ClosedDate");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(followUp);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("FollowUp updated: {Id}", followUp.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FollowUps.Any(e => e.Id == followUp.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Id", followUp.InspectionId);
            return View(followUp);
        }

        // ✅ GET DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var followUp = await _context.FollowUps
                .Include(f => f.Inspection)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (followUp == null)
                return NotFound();

            return View(followUp);
        }

        // ✅ POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);

            if (followUp != null)
            {
                _context.FollowUps.Remove(followUp);
                await _context.SaveChangesAsync();

                _logger.LogWarning("FollowUp deleted: {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}