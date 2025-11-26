using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dcar.Data;
using Dcar.Models;

namespace Dcar.Controllers
{
    public class CombustivelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CombustivelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Combustivels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Combustivels.ToListAsync());
        }

        // GET: Combustivels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combustivel = await _context.Combustivels
                .FirstOrDefaultAsync(m => m.IdCombustivel == id);
            if (combustivel == null)
            {
                return NotFound();
            }

            return View(combustivel);
        }

        // GET: Combustivels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Combustivels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCombustivel,Tipo")] Combustivel combustivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(combustivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(combustivel);
        }

        // GET: Combustivels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combustivel = await _context.Combustivels.FindAsync(id);
            if (combustivel == null)
            {
                return NotFound();
            }
            return View(combustivel);
        }

        // POST: Combustivels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCombustivel,Tipo")] Combustivel combustivel)
        {
            if (id != combustivel.IdCombustivel)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(combustivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CombustivelExists(combustivel.IdCombustivel))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(combustivel);
        }

        // GET: Combustivels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var combustivel = await _context.Combustivels
                .FirstOrDefaultAsync(m => m.IdCombustivel == id);
            if (combustivel == null)
            {
                return NotFound();
            }

            return View(combustivel);
        }

        // POST: Combustivels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var combustivel = await _context.Combustivels.FindAsync(id);
            if (combustivel != null)
            {
                _context.Combustivels.Remove(combustivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CombustivelExists(int id)
        {
            return _context.Combustivels.Any(e => e.IdCombustivel == id);
        }
    }
}
