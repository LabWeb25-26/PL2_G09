using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DCarMarketplace.Data;
using DCarMarketplace.Models;
using DCarMarketplace.Models.ViewModels; // <--- OBRIGATÓRIO: Para usar o ViewModel
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DCarMarketplace.Controllers
{
    public class AnunciosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizador> _userManager;

        public AnunciosController(ApplicationDbContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Anuncios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Anuncios.Include(a => a.Carro).Include(a => a.Vendedor);
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
                .Include(a => a.Carro)
                .Include(a => a.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            return View(anuncio);
        }

        // =============================================================
        // MÉTODOS ATUALIZADOS PARA USAR VIEW MODEL
        // =============================================================

        // GET: Anuncios/Create
        [Authorize]
        public IActionResult Create()
        {
            // Em vez de pedir CarroId, pedimos Marca, Combustível, etc.
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nome");
            ViewData["CombustivelId"] = new SelectList(_context.Combustiveis, "Id", "Tipo");
            // Se tiveres modelos na BD, isto carrega-os. Se não, a dropdown aparece vazia.
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nome");

            return View();
        }

        // POST: Anuncios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CriarAnuncioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                // 1. CRIAR O CARRO NA BD
                var carro = new Carro
                {
                    Matricula = model.Matricula,
                    Ano = model.Ano,
                    Quilometragem = model.Quilometragem,
                    Caixa = model.Caixa,
                    CombustivelId = model.CombustivelId,
                    ModeloId = model.ModeloId,
                    VIN = "N/A" // Valor por defeito opcional
                };

                _context.Carros.Add(carro);
                await _context.SaveChangesAsync(); // Isto gera o ID do Carro automaticamente

                // 2. CRIAR O ANÚNCIO (Ligado ao Carro e ao User)
                var anuncio = new Anuncio
                {
                    Titulo = model.Titulo,
                    Descricao = model.Descricao,
                    Preco = model.Preco,
                    Localizacao = model.Localizacao,
                    DataInicio = DateTime.Now,
                    Estado = "ativo",
                    VendedorId = user.Id,
                    CarroId = carro.Id // <--- Ligação feita aqui
                };

                _context.Anuncios.Add(anuncio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Se houver erro, recarregar as listas para as dropdowns não ficarem vazias
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nome", model.MarcaId);
            ViewData["CombustivelId"] = new SelectList(_context.Combustiveis, "Id", "Tipo", model.CombustivelId);
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nome", model.ModeloId);

            return View(model);
        }

        // =============================================================
        // FIM DOS MÉTODOS ATUALIZADOS
        // =============================================================

        // GET: Anuncios/Edit/5
        [Authorize]
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

            // Verifica se o anúncio pertence a quem está logado (Segurança)
            var user = await _userManager.GetUserAsync(User);
            if (anuncio.VendedorId != user.Id && !User.IsInRole("Administrador"))
            {
                return Forbid(); // Ou RedirectToAction("Index")
            }

            ViewData["CarroId"] = new SelectList(_context.Carros, "Id", "Matricula", anuncio.CarroId);
            return View(anuncio);
        }

        // POST: Anuncios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VendedorId,CarroId,Titulo,Descricao,DataInicio,DataFim,Localizacao,Preco,Estado,Fotos")] Anuncio anuncio)
        {
            if (id != anuncio.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anuncio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnuncioExists(anuncio.Id))
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
            ViewData["CarroId"] = new SelectList(_context.Carros, "Id", "Matricula", anuncio.CarroId);
            return View(anuncio);
        }

        // GET: Anuncios/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.Carro)
                .Include(a => a.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            return View(anuncio);
        }

        // POST: Anuncios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
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
            return _context.Anuncios.Any(e => e.Id == id);
        }
    }
}