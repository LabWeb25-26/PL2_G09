using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketplaceVeiculos.Data;
using MarketplaceVeiculos.Models;

namespace MarketplaceVeiculos.Controllers
{
    public class CarroesController : Controller
    {
        private readonly MarketplaceContext _context;

        public CarroesController(MarketplaceContext context)
        {
            _context = context;
        }

        // GET: Carroes
        public async Task<IActionResult> Index()
        {
            var marketplaceContext = _context.Carros.Include(c => c.Combustivel).Include(c => c.Modelo);
            return View(await marketplaceContext.ToListAsync());
        }

        // GET: Carroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carro = await _context.Carros
                .Include(c => c.Combustivel)
                .Include(c => c.Modelo)
                .FirstOrDefaultAsync(m => m.id_carro == id);
            if (carro == null)
            {
                return NotFound();
            }

            return View(carro);
        }

        // GET: Carroes/Create
        public IActionResult Create()
        {
            // Vai buscar a lista de Combustíveis (Valor=ID, Texto=Tipo)
            ViewData["id_combustivel"] = new SelectList(_context.Combustiveis, "id_combustivel", "tipo");

            // Vai buscar a lista de Modelos (Valor=ID, Texto=Nome)
            ViewData["id_modelo"] = new SelectList(_context.Modelos, "id_modelo", "nome_modelo");

            return View();
        }

        // POST: Carroes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id_carro,VIN,matricula,ano,quilometragem,caixa,id_modelo,id_combustivel")] Carro carro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["id_combustivel"] = new SelectList(_context.Combustiveis, "id_combustivel", "id_combustivel", carro.id_combustivel);
            ViewData["id_modelo"] = new SelectList(_context.Modelos, "id_modelo", "id_modelo", carro.id_modelo);
            return View(carro);
        }

        // GET: Carroes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carro = await _context.Carros.FindAsync(id);
            if (carro == null)
            {
                return NotFound();
            }
            ViewData["id_combustivel"] = new SelectList(_context.Combustiveis, "id_combustivel", "id_combustivel", carro.id_combustivel);
            ViewData["id_modelo"] = new SelectList(_context.Modelos, "id_modelo", "id_modelo", carro.id_modelo);
            return View(carro);
        }

        // POST: Carroes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id_carro,VIN,matricula,ano,quilometragem,caixa,id_modelo,id_combustivel")] Carro carro)
        {
            if (id != carro.id_carro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarroExists(carro.id_carro))
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
            ViewData["id_combustivel"] = new SelectList(_context.Combustiveis, "id_combustivel", "id_combustivel", carro.id_combustivel);
            ViewData["id_modelo"] = new SelectList(_context.Modelos, "id_modelo", "id_modelo", carro.id_modelo);
            return View(carro);
        }

        // GET: Carroes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carro = await _context.Carros
                .Include(c => c.Combustivel)
                .Include(c => c.Modelo)
                .FirstOrDefaultAsync(m => m.id_carro == id);
            if (carro == null)
            {
                return NotFound();
            }

            return View(carro);
        }

        // POST: Carroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carro = await _context.Carros.FindAsync(id);
            if (carro != null)
            {
                _context.Carros.Remove(carro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarroExists(int id)
        {
            return _context.Carros.Any(e => e.id_carro == id);
        }
    }
}
