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
    public class CarroesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarroesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carroes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Carros
                .Include(c => c.IdCombustivelNavigation)
                .Include(c => c.IdModeloNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Carroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carro = await _context.Carros
                .Include(c => c.IdCombustivelNavigation)
                .Include(c => c.IdModeloNavigation)
                .FirstOrDefaultAsync(m => m.IdCarro == id);
            if (carro == null)
            {
                return NotFound();
            }

            return View(carro);
        }

        // GET: Carroes/Create
        public IActionResult Create()
        {
            // CORREÇÃO: Mostrar nomes nas listas em vez de IDs
            ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "Tipo");
            ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "NomeModelo");
            return View();
        }

        // POST: Carroes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCarro,Vin,Matricula,Ano,Quilometragem,Caixa,IdModelo,IdCombustivel")] Carro carro)
        {
            // CORREÇÃO: Remover validações que bloqueiam a gravação
            ModelState.Remove("IdCombustivelNavigation");
            ModelState.Remove("IdModeloNavigation");
            ModelState.Remove("Anuncio"); // O carro novo ainda não tem anúncio

            if (ModelState.IsValid)
            {
                _context.Add(carro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se falhar, recarrega as listas com os nomes corretos
            ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "Tipo", carro.IdCombustivel);
            ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "NomeModelo", carro.IdModelo);
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
            // CORREÇÃO NO EDIT TAMBÉM
            ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "Tipo", carro.IdCombustivel);
            ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "NomeModelo", carro.IdModelo);
            return View(carro);
        }

        // POST: Carroes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCarro,Vin,Matricula,Ano,Quilometragem,Caixa,IdModelo,IdCombustivel")] Carro carro)
        {
            if (id != carro.IdCarro)
            {
                return NotFound();
            }

            ModelState.Remove("IdCombustivelNavigation");
            ModelState.Remove("IdModeloNavigation");
            ModelState.Remove("Anuncio");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarroExists(carro.IdCarro))
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
            ViewData["IdCombustivel"] = new SelectList(_context.Combustivels, "IdCombustivel", "Tipo", carro.IdCombustivel);
            ViewData["IdModelo"] = new SelectList(_context.Modelos, "IdModelo", "NomeModelo", carro.IdModelo);
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
                .Include(c => c.IdCombustivelNavigation)
                .Include(c => c.IdModeloNavigation)
                .FirstOrDefaultAsync(m => m.IdCarro == id);
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
            return _context.Carros.Any(e => e.IdCarro == id);
        }
    }
}