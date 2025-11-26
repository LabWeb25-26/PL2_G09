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
    public class ModeloesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModeloesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Modeloes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Modelos.Include(m => m.IdMarcaNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Modeloes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelo = await _context.Modelos
                .Include(m => m.IdMarcaNavigation)
                .FirstOrDefaultAsync(m => m.IdModelo == id);
            if (modelo == null)
            {
                return NotFound();
            }

            return View(modelo);
        }

        // GET: Modeloes/Create
        public IActionResult Create()
        {
            // ALTERAÇÃO 1: Mudei o último argumento para "NomeMarca" para aparecer o texto bonito na lista
            ViewData["IdMarca"] = new SelectList(_context.Marcas, "IdMarca", "NomeMarca");
            return View();
        }

        // POST: Modeloes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdModelo,IdMarca,NomeModelo")] Modelo modelo)
        {
            // ALTERAÇÃO 2: Ignorar a validação da tabela Pai para não dar erro ao gravar
            ModelState.Remove("IdMarcaNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(modelo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se falhar, recarrega a lista com os Nomes
            ViewData["IdMarca"] = new SelectList(_context.Marcas, "IdMarca", "NomeMarca", modelo.IdMarca);
            return View(modelo);
        }

        // GET: Modeloes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelo = await _context.Modelos.FindAsync(id);
            if (modelo == null)
            {
                return NotFound();
            }
            // ALTERAÇÃO 3: Também corrigi no Edit para mostrar nomes
            ViewData["IdMarca"] = new SelectList(_context.Marcas, "IdMarca", "NomeMarca", modelo.IdMarca);
            return View(modelo);
        }

        // POST: Modeloes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdModelo,IdMarca,NomeModelo")] Modelo modelo)
        {
            if (id != modelo.IdModelo)
            {
                return NotFound();
            }

            // ALTERAÇÃO 4: Ignorar a validação da tabela Pai no Edit também
            ModelState.Remove("IdMarcaNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModeloExists(modelo.IdModelo))
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
            ViewData["IdMarca"] = new SelectList(_context.Marcas, "IdMarca", "NomeMarca", modelo.IdMarca);
            return View(modelo);
        }

        // GET: Modeloes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelo = await _context.Modelos
                .Include(m => m.IdMarcaNavigation)
                .FirstOrDefaultAsync(m => m.IdModelo == id);
            if (modelo == null)
            {
                return NotFound();
            }

            return View(modelo);
        }

        // POST: Modeloes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modelo = await _context.Modelos.FindAsync(id);
            if (modelo != null)
            {
                _context.Modelos.Remove(modelo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModeloExists(int id)
        {
            return _context.Modelos.Any(e => e.IdModelo == id);
        }
    }
}