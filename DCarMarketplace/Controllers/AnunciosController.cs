using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DCarMarketplace.Data;
using DCarMarketplace.Models;
using DCarMarketplace.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // Essencial para Upload de Ficheiros

namespace DCarMarketplace.Controllers
{
    public class AnunciosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizador> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnunciosController(ApplicationDbContext context,
                                  UserManager<Utilizador> userManager,
                                  IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // =============================================================
        // PÁGINA PRINCIPAL DE ANÚNCIOS (COM PESQUISA)
        // =============================================================
        // GET: Anuncios
        public async Task<IActionResult> Index(string marca, string modelo, string pesquisa, string ordenacao)
        {
            // Carregar marcas para a dropdown de filtros
            ViewBag.ListaMarcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();

            // CARREGAR FAVORITOS DO USER (Se logado)
            ViewBag.FavoritosIds = new List<int>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.FavoritosIds = await _context.AnunciosFavoritos
                    .Where(f => f.UtilizadorId == userId)
                    .Select(f => f.AnuncioId)
                    .ToListAsync();
            }

            var query = _context.Anuncios
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Modelo)
                        .ThenInclude(m => m.Marca)
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Combustivel)
                .Include(a => a.Vendedor)
                    .ThenInclude(v => v.Utilizador)
                .Where(a => a.Estado == "ativo")
                .AsQueryable();

            // Filtros
            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(a => a.Carro.Modelo.Marca.Nome == marca);
            }

            if (!string.IsNullOrEmpty(modelo))
            {
                query = query.Where(a => a.Carro.Modelo.Nome == modelo);
            }

            if (!string.IsNullOrEmpty(pesquisa))
            {
                query = query.Where(a => a.Titulo.Contains(pesquisa) ||
                                         a.Carro.Modelo.Nome.Contains(pesquisa));
            }

            // Ordenação
            switch (ordenacao)
            {
                case "preco_desc": query = query.OrderByDescending(a => a.Preco); break;
                case "preco_asc": query = query.OrderBy(a => a.Preco); break;
                case "recente":
                default: query = query.OrderByDescending(a => a.DataInicio); break;
            }

            ViewData["MarcaAtual"] = marca;
            ViewData["ModeloAtual"] = modelo;
            ViewData["PesquisaAtual"] = pesquisa;
            ViewData["OrdenacaoAtual"] = ordenacao;

            return View(await query.ToListAsync());
        }

        // GET: Anuncios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var anuncio = await _context.Anuncios
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Modelo)
                        .ThenInclude(m => m.Marca)
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Combustivel)
                .Include(a => a.Vendedor)
                    .ThenInclude(v => v.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (anuncio == null) return NotFound();

            return View(anuncio);
        }

        // =============================================================
        // ÁREA DO VENDEDOR (OS MEUS ANÚNCIOS)
        // =============================================================
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> MeusAnuncios()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var meusAnuncios = await _context.Anuncios
                .Include(a => a.Carro)
                .Include(a => a.Vendedor)
                    .ThenInclude(v => v.Utilizador)
                .Where(a => a.VendedorId == user.Id)
                .ToListAsync();

            ViewData["Title"] = "A Minha Garagem";
            return View("Index", meusAnuncios);
        }

        // =============================================================
        // GESTÃO (CRIAR, EDITAR, APAGAR)
        // =============================================================

        // GET: Anuncios/Create
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedores.FindAsync(user.Id);

            if (vendedor == null || vendedor.EstadoAprovacao != "aprovado")
            {
                return View("AguardandoAprovacao");
            }

            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nome");
            ViewData["CombustivelId"] = new SelectList(_context.Combustiveis, "Id", "Tipo");
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nome");
            ViewData["ListaAnos"] = new SelectList(GerarListaAnos());

            return View();
        }

        // POST: Anuncios/Create (COM UPLOAD MÚLTIPLO)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Create(CriarAnuncioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                string stringNomesImagens = null;
                List<string> listaNomes = new List<string>();

                if (model.ImagensFicheiros != null && model.ImagensFicheiros.Count > 0)
                {
                    if (model.ImagensFicheiros.Count > 9)
                    {
                        ModelState.AddModelError("ImagensFicheiros", "Máximo 9 fotos permitidas.");
                        RecarregarListas(model);
                        return View(model);
                    }

                    string pastaUpload = Path.Combine(_webHostEnvironment.WebRootPath, "imagens_carros");
                    if (!Directory.Exists(pastaUpload)) Directory.CreateDirectory(pastaUpload);

                    foreach (var ficheiro in model.ImagensFicheiros)
                    {
                        if (ficheiro.Length > 0)
                        {
                            string nomeUnico = Guid.NewGuid().ToString() + Path.GetExtension(ficheiro.FileName);
                            string caminhoCompleto = Path.Combine(pastaUpload, nomeUnico);

                            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                            {
                                await ficheiro.CopyToAsync(stream);
                            }
                            listaNomes.Add(nomeUnico);
                        }
                    }
                    stringNomesImagens = string.Join(";", listaNomes);
                }

                var carro = new Carro
                {
                    Matricula = model.Matricula,
                    Ano = model.Ano,
                    Quilometragem = model.Quilometragem,
                    Caixa = model.Caixa,
                    CombustivelId = model.CombustivelId,
                    ModeloId = model.ModeloId,
                    VIN = "N/A"
                };
                _context.Carros.Add(carro);
                await _context.SaveChangesAsync();

                var anuncio = new Anuncio
                {
                    Titulo = model.Titulo,
                    Descricao = model.Descricao,
                    Preco = model.Preco,
                    Localizacao = model.Localizacao,
                    DataInicio = DateTime.Now,
                    Estado = "ativo",
                    VendedorId = user.Id,
                    CarroId = carro.Id,
                    Fotos = stringNomesImagens
                };
                _context.Anuncios.Add(anuncio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MeusAnuncios));
            }

            RecarregarListas(model);
            return View(model);
        }

        private void RecarregarListas(CriarAnuncioViewModel model)
        {
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nome", model.MarcaId);
            ViewData["CombustivelId"] = new SelectList(_context.Combustiveis, "Id", "Tipo", model.CombustivelId);
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nome", model.ModeloId);
            ViewData["ListaAnos"] = new SelectList(GerarListaAnos(), model.Ano);
        }

        // GET: Anuncios/Edit/5
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (anuncio.VendedorId != user.Id && !User.IsInRole("Administrador")) return Forbid();

            ViewData["CarroId"] = new SelectList(_context.Carros, "Id", "Matricula", anuncio.CarroId);
            return View(anuncio);
        }

        // POST: Anuncios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VendedorId,CarroId,Titulo,Descricao,DataInicio,DataFim,Localizacao,Preco,Estado,Fotos")] Anuncio anuncio)
        {
            if (id != anuncio.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anuncio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnuncioExists(anuncio.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(MeusAnuncios));
            }
            return View(anuncio);
        }

        // GET: Anuncios/Delete/5
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var anuncio = await _context.Anuncios
                .Include(a => a.Carro)
                .Include(a => a.Vendedor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (anuncio == null) return NotFound();
            return View(anuncio);
        }

        // POST: Anuncios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio != null) _context.Anuncios.Remove(anuncio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MeusAnuncios));
        }

        private bool AnuncioExists(int id)
        {
            return _context.Anuncios.Any(e => e.Id == id);
        }

        // =============================================================
        // APIs AJAX
        // =============================================================

        private List<int> GerarListaAnos()
        {
            var anos = new List<int>();
            for (int i = 1900; i < 1990; i += 10) anos.Add(i);
            for (int i = 1990; i <= 2026; i++) anos.Add(i);
            anos.Reverse();
            return anos;
        }

        [HttpGet]
        public async Task<JsonResult> GetModelosByMarca(string nomeMarca)
        {
            var modelos = await _context.Modelos
                .Where(m => m.Marca.Nome == nomeMarca)
                .OrderBy(m => m.Nome)
                .Select(m => new { id = m.Nome, nome = m.Nome })
                .ToListAsync();
            return Json(modelos);
        }

        [HttpGet]
        public async Task<JsonResult> GetModelosByMarcaId(int marcaId)
        {
            var modelos = await _context.Modelos
                .Where(m => m.MarcaId == marcaId)
                .OrderBy(m => m.Nome)
                .Select(m => new { id = m.Id, nome = m.Nome })
                .ToListAsync();
            return Json(modelos);
        }

        [HttpGet]
        public async Task<JsonResult> GetContagemAnuncios(string marca, string modelo)
        {
            var query = _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Where(a => a.Estado == "ativo")
                .AsQueryable();

            if (!string.IsNullOrEmpty(marca))
                query = query.Where(a => a.Carro.Modelo.Marca.Nome == marca);

            if (!string.IsNullOrEmpty(modelo) && modelo != "Modelo" && modelo != "Todos os modelos")
                query = query.Where(a => a.Carro.Modelo.Nome == modelo);

            int total = await query.CountAsync();
            return Json(total);
        }
    }
}