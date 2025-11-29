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
using Microsoft.AspNetCore.Hosting;
using System.IO;

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
        // 1. LISTAGEM PÚBLICA (COM FILTROS AVANÇADOS)
        // =============================================================
        public async Task<IActionResult> Index(
            string marca, string modelo, string pesquisa, string ordenacao,
            int? precoMin, int? precoMax, int? anoMin, int? anoMax,
            int? kmMin, int? kmMax, string combustivel, string caixa,
            string segmento, string localizacao)
        {
            ViewBag.ListaMarcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();

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
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Include(a => a.Carro).ThenInclude(c => c.Combustivel)
                .Include(a => a.Vendedor).ThenInclude(v => v.Utilizador)
                .Where(a => a.Estado == "ativo")
                .AsQueryable();

            // --- FILTROS ---
            if (!string.IsNullOrEmpty(marca)) query = query.Where(a => a.Carro.Modelo.Marca.Nome == marca);
            if (!string.IsNullOrEmpty(modelo)) query = query.Where(a => a.Carro.Modelo.Nome == modelo);

            if (!string.IsNullOrEmpty(pesquisa))
            {
                query = query.Where(a => a.Titulo.Contains(pesquisa) ||
                                         a.Carro.Modelo.Nome.Contains(pesquisa) ||
                                         a.Carro.Modelo.Marca.Nome.Contains(pesquisa));
            }

            if (precoMin.HasValue) query = query.Where(a => a.Preco >= precoMin.Value);
            if (precoMax.HasValue) query = query.Where(a => a.Preco <= precoMax.Value);
            if (anoMin.HasValue) query = query.Where(a => a.Carro.Ano >= anoMin.Value);
            if (anoMax.HasValue) query = query.Where(a => a.Carro.Ano <= anoMax.Value);
            if (kmMin.HasValue) query = query.Where(a => a.Carro.Quilometragem >= kmMin.Value);
            if (kmMax.HasValue) query = query.Where(a => a.Carro.Quilometragem <= kmMax.Value);

            if (!string.IsNullOrEmpty(combustivel) && combustivel != "Todos") query = query.Where(a => a.Carro.Combustivel.Tipo == combustivel);
            if (!string.IsNullOrEmpty(caixa) && caixa != "Todas") query = query.Where(a => a.Carro.Caixa == caixa);
            if (!string.IsNullOrEmpty(segmento) && segmento != "Todos") query = query.Where(a => a.Carro.Segmento == segmento);
            if (!string.IsNullOrEmpty(localizacao)) query = query.Where(a => a.Localizacao.Contains(localizacao));

            // --- ORDENAÇÃO ---
            switch (ordenacao)
            {
                case "preco_desc": query = query.OrderByDescending(a => a.Preco); break;
                case "preco_asc": query = query.OrderBy(a => a.Preco); break;
                case "km_asc": query = query.OrderBy(a => a.Carro.Quilometragem); break;
                case "km_desc": query = query.OrderByDescending(a => a.Carro.Quilometragem); break;
                case "recente": default: query = query.OrderByDescending(a => a.DataInicio); break;
            }

            // Manter estado na View
            ViewData["MarcaAtual"] = marca;
            ViewData["ModeloAtual"] = modelo;
            ViewData["PesquisaAtual"] = pesquisa;
            ViewData["OrdenacaoAtual"] = ordenacao;
            ViewData["PrecoMin"] = precoMin;
            ViewData["PrecoMax"] = precoMax;
            ViewData["AnoMin"] = anoMin;
            ViewData["AnoMax"] = anoMax;
            ViewData["KmMin"] = kmMin;
            ViewData["KmMax"] = kmMax;
            ViewData["CombustivelAtual"] = combustivel;
            ViewData["CaixaAtual"] = caixa;
            ViewData["SegmentoAtual"] = segmento;
            ViewData["LocalizacaoAtual"] = localizacao;

            return View(await query.ToListAsync());
        }

        // =============================================================
        // 2. DETALHES DO ANÚNCIO
        // =============================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var anuncio = await _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Include(a => a.Carro).ThenInclude(c => c.Combustivel)
                .Include(a => a.Vendedor).ThenInclude(v => v.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (anuncio == null) return NotFound();

            return View(anuncio);
        }

        // =============================================================
        // 3. MEUS ANÚNCIOS (VENDEDOR)
        // =============================================================
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> MeusAnuncios()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var meusAnuncios = await _context.Anuncios
                .Include(a => a.Carro)
                .Include(a => a.Vendedor).ThenInclude(v => v.Utilizador)
                .Where(a => a.VendedorId == user.Id)
                .ToListAsync();

            ViewData["Title"] = "A Minha Garagem";
            return View("Index", meusAnuncios);
        }

        // =============================================================
        // 4. CRIAR ANÚNCIO
        // =============================================================
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendedor = await _context.Vendedores.FindAsync(user.Id);

            if (vendedor == null || vendedor.EstadoAprovacao != "aprovado")
            {
                return View("AguardandoAprovacao");
            }

            CarregarListasViewData();
            return View();
        }

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

                // Upload de Imagens
                if (model.ImagensFicheiros != null && model.ImagensFicheiros.Count > 0)
                {
                    if (model.ImagensFicheiros.Count > 9)
                    {
                        ModelState.AddModelError("ImagensFicheiros", "Máximo 9 fotos permitidas.");
                        CarregarListasViewData(model);
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

                // Criar Carro
                var carro = new Carro
                {
                    Matricula = model.Matricula,
                    Ano = model.Ano,
                    Quilometragem = model.Quilometragem,
                    Caixa = model.Caixa,
                    CombustivelId = model.CombustivelId,
                    ModeloId = model.ModeloId,
                    VIN = model.VIN,
                    Cor = model.Cor,
                    NumeroPortas = model.NumeroPortas,
                    Segmento = model.Segmento,
                    Potencia = model.Potencia,
                    Cilindrada = model.Cilindrada
                };
                _context.Carros.Add(carro);
                await _context.SaveChangesAsync();

                // Criar Anúncio
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

            CarregarListasViewData(model);
            return View(model);
        }

        private void CarregarListasViewData(CriarAnuncioViewModel model = null)
        {
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nome", model?.MarcaId);
            ViewData["CombustivelId"] = new SelectList(_context.Combustiveis, "Id", "Tipo", model?.CombustivelId);
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nome", model?.ModeloId);
            ViewData["ListaAnos"] = new SelectList(GerarListaAnos(), model?.Ano);
        }
        // =============================================================
        // LISTAGENS ESPECÍFICAS (RF-20 e RF-21)
        // =============================================================

        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> VeiculosReservados()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var anuncios = await _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Include(a => a.Carro).ThenInclude(c => c.Combustivel)
                .Include(a => a.Vendedor).ThenInclude(v => v.Utilizador)
                .Where(a => a.VendedorId == user.Id && a.Estado == "reservado")
                .ToListAsync();

            ViewData["Title"] = "Veículos Reservados";
            return View("Index", anuncios); // Reutiliza a View Index
        }

        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> VeiculosVendidos()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var anuncios = await _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Include(a => a.Carro).ThenInclude(c => c.Combustivel)
                .Include(a => a.Vendedor).ThenInclude(v => v.Utilizador)
                .Where(a => a.VendedorId == user.Id && a.Estado == "vendido")
                .ToListAsync();

            ViewData["Title"] = "Histórico de Vendas";
            return View("Index", anuncios); // Reutiliza a View Index
        }
        // =============================================================
        // 5. EDITAR ANÚNCIO
        // =============================================================
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var anuncio = await _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anuncio == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (anuncio.VendedorId != user.Id && !User.IsInRole("Administrador")) return Forbid();

            // Converter para ViewModel
            var viewModel = new EditarAnuncioViewModel
            {
                Id = anuncio.Id,
                Titulo = anuncio.Titulo,
                Descricao = anuncio.Descricao,
                Preco = anuncio.Preco,
                Localizacao = anuncio.Localizacao,
                FotosAtuais = anuncio.Fotos,
                Matricula = anuncio.Carro.Matricula,
                VIN = anuncio.Carro.VIN,
                Ano = anuncio.Carro.Ano,
                Quilometragem = anuncio.Carro.Quilometragem,
                Caixa = anuncio.Carro.Caixa,
                Cor = anuncio.Carro.Cor,
                NumeroPortas = anuncio.Carro.NumeroPortas,
                Segmento = anuncio.Carro.Segmento,
                Potencia = anuncio.Carro.Potencia,
                Cilindrada = anuncio.Carro.Cilindrada,
                MarcaId = anuncio.Carro.Modelo.MarcaId,
                ModeloId = anuncio.Carro.ModeloId,
                CombustivelId = anuncio.Carro.CombustivelId
            };

            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nome", viewModel.MarcaId);
            ViewData["ModeloId"] = new SelectList(_context.Modelos.Where(m => m.MarcaId == viewModel.MarcaId), "Id", "Nome", viewModel.ModeloId);
            ViewData["CombustivelId"] = new SelectList(_context.Combustiveis, "Id", "Tipo", viewModel.CombustivelId);
            ViewData["ListaAnos"] = new SelectList(GerarListaAnos(), viewModel.Ano);

            ViewBag.InfoCarro = $"{anuncio.Carro.Modelo?.Marca?.Nome} {anuncio.Carro.Modelo?.Nome} ({anuncio.Carro.Ano})";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Edit(int id, EditarAnuncioViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var anuncioOriginal = await _context.Anuncios
                    .Include(a => a.Carro)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (anuncioOriginal == null) return NotFound();

                // Atualizar Anúncio
                anuncioOriginal.Titulo = model.Titulo;
                anuncioOriginal.Descricao = model.Descricao;
                anuncioOriginal.Preco = model.Preco;
                anuncioOriginal.Localizacao = model.Localizacao;

                // Atualizar Carro
                if (anuncioOriginal.Carro != null)
                {
                    anuncioOriginal.Carro.Matricula = model.Matricula;
                    anuncioOriginal.Carro.VIN = model.VIN;
                    anuncioOriginal.Carro.Ano = model.Ano;
                    anuncioOriginal.Carro.Quilometragem = model.Quilometragem;
                    anuncioOriginal.Carro.Caixa = model.Caixa;
                    anuncioOriginal.Carro.Cor = model.Cor;
                    anuncioOriginal.Carro.NumeroPortas = model.NumeroPortas;
                    anuncioOriginal.Carro.Segmento = model.Segmento;
                    anuncioOriginal.Carro.Potencia = model.Potencia;
                    anuncioOriginal.Carro.Cilindrada = model.Cilindrada;
                    anuncioOriginal.Carro.CombustivelId = model.CombustivelId;
                    anuncioOriginal.Carro.ModeloId = model.ModeloId;
                }

                // Adicionar Novas Fotos
                if (model.NovasFotos != null && model.NovasFotos.Count > 0)
                {
                    string pastaUpload = Path.Combine(_webHostEnvironment.WebRootPath, "imagens_carros");
                    if (!Directory.Exists(pastaUpload)) Directory.CreateDirectory(pastaUpload);

                    List<string> nomesNovos = new List<string>();
                    foreach (var ficheiro in model.NovasFotos)
                    {
                        if (ficheiro.Length > 0)
                        {
                            string nomeUnico = Guid.NewGuid().ToString() + Path.GetExtension(ficheiro.FileName);
                            string caminhoCompleto = Path.Combine(pastaUpload, nomeUnico);
                            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                            {
                                await ficheiro.CopyToAsync(stream);
                            }
                            nomesNovos.Add(nomeUnico);
                        }
                    }

                    if (!string.IsNullOrEmpty(anuncioOriginal.Fotos))
                        anuncioOriginal.Fotos += ";" + string.Join(";", nomesNovos);
                    else
                        anuncioOriginal.Fotos = string.Join(";", nomesNovos);
                }

                try
                {
                    _context.Update(anuncioOriginal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnuncioExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(MeusAnuncios));
            }

            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nome", model.MarcaId);
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nome", model.ModeloId);
            ViewData["CombustivelId"] = new SelectList(_context.Combustiveis, "Id", "Tipo", model.CombustivelId);
            ViewData["ListaAnos"] = new SelectList(GerarListaAnos(), model.Ano);

            return View(model);
        }

        // =============================================================
        // 6. APAGAR ANÚNCIO (CORRIGIDO)
        // =============================================================
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // IMPORTANTE: Include para não dar erro na página de Delete
            var anuncio = await _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Include(a => a.Vendedor).ThenInclude(v => v.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (anuncio == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (anuncio.VendedorId != user.Id && !User.IsInRole("Administrador")) return Forbid();

            return View(anuncio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anuncio = await _context.Anuncios.Include(a => a.Carro).FirstOrDefaultAsync(a => a.Id == id);

            if (anuncio != null)
            {
                // 1. Apagar Favoritos e Reservas antes para evitar erro de FK
                var favoritos = _context.AnunciosFavoritos.Where(f => f.AnuncioId == id);
                _context.AnunciosFavoritos.RemoveRange(favoritos);

                var reservas = _context.Reservas.Where(r => r.AnuncioId == id);
                _context.Reservas.RemoveRange(reservas);

                // 2. Apagar Carro e Anúncio
                _context.Carros.Remove(anuncio.Carro);
                _context.Anuncios.Remove(anuncio);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MeusAnuncios));
        }

        // =============================================================
        // 7. GESTÃO DE ESTADO (A TUA FUNCIONALIDADE)
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> AtualizarEstado(int id, string novoEstado)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (anuncio.VendedorId != user.Id && !User.IsInRole("Administrador")) return Forbid();

            if (new[] { "ativo", "reservado", "vendido", "pausado" }.Contains(novoEstado))
            {
                anuncio.Estado = novoEstado;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MeusAnuncios));
        }

        // =============================================================
        // 8. APIs AJAX e UTILITÁRIOS
        // =============================================================

        private bool AnuncioExists(int id) => _context.Anuncios.Any(e => e.Id == id);

        private List<int> GerarListaAnos()
        {
            var anos = new List<int>();
            for (int i = 1900; i < 1990; i += 10) anos.Add(i);
            for (int i = 1990; i <= 2026; i++) anos.Add(i);
            anos.Reverse();
            return anos;
        }

        [HttpPost]
        [Authorize(Roles = "Vendedor")]
        public async Task<IActionResult> RemoverFotoExistente(int id, string nomeFoto)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio == null) return Json(new { success = false, message = "Anúncio não encontrado" });

            var user = await _userManager.GetUserAsync(User);
            if (anuncio.VendedorId != user.Id && !User.IsInRole("Administrador")) return Json(new { success = false, message = "Sem permissão" });

            var listaFotos = anuncio.Fotos?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
            if (listaFotos.Contains(nomeFoto))
            {
                listaFotos.Remove(nomeFoto);
                anuncio.Fotos = string.Join(";", listaFotos);

                var caminho = Path.Combine(_webHostEnvironment.WebRootPath, "imagens_carros", nomeFoto);
                if (System.IO.File.Exists(caminho)) System.IO.File.Delete(caminho);

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet] public async Task<JsonResult> GetModelosByMarca(string nomeMarca) { return Json(await _context.Modelos.Where(m => m.Marca.Nome == nomeMarca).OrderBy(m => m.Nome).Select(m => new { id = m.Nome, nome = m.Nome }).ToListAsync()); }
        [HttpGet] public async Task<JsonResult> GetModelosByMarcaId(int marcaId) { return Json(await _context.Modelos.Where(m => m.MarcaId == marcaId).OrderBy(m => m.Nome).Select(m => new { id = m.Id, nome = m.Nome }).ToListAsync()); }
        [HttpGet]
        public async Task<JsonResult> GetContagemAnuncios(string marca, string modelo)
        {
            var query = _context.Anuncios.Where(a => a.Estado == "ativo").AsQueryable();
            if (!string.IsNullOrEmpty(marca)) query = query.Where(a => a.Carro.Modelo.Marca.Nome == marca);
            if (!string.IsNullOrEmpty(modelo) && modelo != "Modelo" && modelo != "Todos os modelos") query = query.Where(a => a.Carro.Modelo.Nome == modelo);
            return Json(await query.CountAsync());
        }
    }
}