using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dcar.Data;
using Dcar.Models;
using System.IO; // Necessário para o Upload de Imagens

namespace Dcar.Controllers
{
    public class AnunciosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnunciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Anuncios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Anuncios
                .Include(a => a.IdCarroNavigation)
                .Include(a => a.IdVendedorNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Anuncios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.IdCarroNavigation)
                .Include(a => a.IdVendedorNavigation)
                .FirstOrDefaultAsync(m => m.IdAnuncio == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            return View(anuncio);
        }

        // GET: Anuncios/Create
        public IActionResult Create()
        {
            ViewData["IdCarro"] = new SelectList(_context.Carros, "IdCarro", "Matricula");
            ViewData["IdVendedor"] = new SelectList(_context.Vendedors, "IdVendedor", "Nome");
            return View();
        }

        // POST: Anuncios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAnuncio,IdVendedor,IdCarro,Titulo,Descricao,DataInicio,DataFim,Localizacao,Preco,Estado")] Anuncio anuncio, IFormFile ficheiroFoto)
        {
            // Remover validações automáticas
            ModelState.Remove("IdCarroNavigation");
            ModelState.Remove("IdVendedorNavigation");
            ModelState.Remove("Compra");
            ModelState.Remove("Reservas");
            ModelState.Remove("Agenda");
            ModelState.Remove("Denuncia");
            ModelState.Remove("Fotos");

            if (ModelState.IsValid)
            {
                // Lógica de Upload
                if (ficheiroFoto != null && ficheiroFoto.Length > 0)
                {
                    var nomeFicheiro = Guid.NewGuid().ToString() + Path.GetExtension(ficheiroFoto.FileName);
                    var caminhoPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens");

                    if (!Directory.Exists(caminhoPasta))
                    {
                        Directory.CreateDirectory(caminhoPasta);
                    }

                    var caminhoCompleto = Path.Combine(caminhoPasta, nomeFicheiro);

                    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await ficheiroFoto.CopyToAsync(stream);
                    }

                    anuncio.Fotos = "/imagens/" + nomeFicheiro;
                }
                else
                {
                    anuncio.Fotos = null;
                }

                _context.Add(anuncio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCarro"] = new SelectList(_context.Carros, "IdCarro", "Matricula", anuncio.IdCarro);
            ViewData["IdVendedor"] = new SelectList(_context.Vendedors, "IdVendedor", "Nome", anuncio.IdVendedor);
            return View(anuncio);
        }

        // GET: Anuncios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio == null)
            {
                return NotFound();
            }
            ViewData["IdCarro"] = new SelectList(_context.Carros, "IdCarro", "Matricula", anuncio.IdCarro);
            ViewData["IdVendedor"] = new SelectList(_context.Vendedors, "IdVendedor", "Nome", anuncio.IdVendedor);
            return View(anuncio);
        }

        // POST: Anuncios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAnuncio,IdVendedor,IdCarro,Titulo,Descricao,DataInicio,DataFim,Localizacao,Preco,Estado,Fotos")] Anuncio anuncio)
        {
            if (id != anuncio.IdAnuncio)
            {
                return NotFound();
            }

            ModelState.Remove("IdCarroNavigation");
            ModelState.Remove("IdVendedorNavigation");
            ModelState.Remove("Compra");
            ModelState.Remove("Reservas");
            ModelState.Remove("Agenda");
            ModelState.Remove("Denuncia");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anuncio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnuncioExists(anuncio.IdAnuncio))
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
            ViewData["IdCarro"] = new SelectList(_context.Carros, "IdCarro", "Matricula", anuncio.IdCarro);
            ViewData["IdVendedor"] = new SelectList(_context.Vendedors, "IdVendedor", "Nome", anuncio.IdVendedor);
            return View(anuncio);
        }

        // GET: Anuncios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.IdCarroNavigation)
                .Include(a => a.IdVendedorNavigation)
                .FirstOrDefaultAsync(m => m.IdAnuncio == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            return View(anuncio);
        }

        // POST: Anuncios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio != null)
            {
                _context.Anuncios.Remove(anuncio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnuncioExists(int id)
        {
            return _context.Anuncios.Any(e => e.IdAnuncio == id);
        }
    }
}