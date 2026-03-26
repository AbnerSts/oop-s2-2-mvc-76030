using FoodSafety.Domain.Models;
using FoodSafety.mvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FoodSafety.mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PremisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PremisesController> _logger;

        public PremisesController(ApplicationDbContext context, ILogger<PremisesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ LIST
        public async Task<IActionResult> Index()
        {
            return View(await _context.Premises.ToListAsync());
        }

        // ✅ DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var premises = await _context.Premises
                .FirstOrDefaultAsync(m => m.Id == id);

            if (premises == null)
                return NotFound();

            return View(premises);
        }

        // ✅ GET CREATE
        public IActionResult Create()
        {
            return View();
        }

        // ✅ POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Premises premises)
        {
            if (ModelState.IsValid)
            {
                _context.Add(premises);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Premises created: {Name} in {Town}", premises.Name, premises.Town);

                return RedirectToAction(nameof(Index));
            }

            return View(premises);
        }

        // ✅ GET EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var premises = await _context.Premises.FindAsync(id);

            if (premises == null)
                return NotFound();

            return View(premises);
        }

        // ✅ POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Premises premises)
        {
            if (id != premises.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(premises);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Premises updated: {Id}", premises.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Premises.Any(e => e.Id == premises.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(premises);
        }

        // ✅ GET DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var premises = await _context.Premises
                .FirstOrDefaultAsync(m => m.Id == id);

            if (premises == null)
                return NotFound();

            return View(premises);
        }

        // ✅ POST DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var premises = await _context.Premises.FindAsync(id);

            if (premises != null)
            {
                _context.Premises.Remove(premises);
                await _context.SaveChangesAsync();

                _logger.LogWarning("Premises deleted: {Id}", id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}