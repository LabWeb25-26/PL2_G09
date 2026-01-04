using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DCarMarketplace.Data;
using DCarMarketplace.Models;
using DCarMarketplace.Models.ViewModels;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization; // Necessário para a formatação de texto
                                                                                                                                                                                                                                                                                                   
namespace DCarMarketplace.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizador> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =============================================================
        // 1. DASHBOARD (ESTATÍSTICAS GERAIS)
        // =============================================================
        public async Task<IActionResult> Dashboard()
        {
            int totalUsers = await _context.Users.CountAsync();
            int totalVendedores = await _context.Vendedores.CountAsync();
            int totalCompradores = await _context.Compradores.CountAsync();

            int ativos = await _context.Anuncios.CountAsync(a => a.Estado == "ativo");
            int vendidos = await _context.Anuncios.CountAsync(a => a.Estado == "vendido");

            int numVendas = await _context.Compras.CountAsync();
            decimal valorTotal = 0;
            if (numVendas > 0)
            {
                valorTotal = await _context.Compras.SumAsync(c => c.Preco);
            }

            var topMarcas = await _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .GroupBy(a => a.Carro.Modelo.Marca.Nome)
                .Select(g => new { Marca = g.Key, Quantidade = g.Count() })
                .OrderByDescending(x => x.Quantidade)
                .Take(5)
                .ToDictionaryAsync(x => x.Marca, x => x.Quantidade);

            var model = new AdminDashboardViewModel
            {
                TotalUtilizadores = totalUsers,
                TotalVendedores = totalVendedores,
                TotalCompradores = totalCompradores,
                AnunciosAtivos = ativos,
                AnunciosVendidos = vendidos,
                TotalVendasRealizadas = numVendas,
                ValorTotalTransacionado = valorTotal,
                TopMarcas = topMarcas
            };

            return View(model);
        }

        // =============================================================
        // 2. LISTA DE UTILIZADORES
        // =============================================================
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Vendedor)
                .Include(u => u.Comprador)
                .OrderByDescending(u => u.DataRegisto)
                .ToListAsync();

            return View(users);
        }

        // =============================================================
        // 3. CRIAR NOVO ADMINISTRADOR
        // =============================================================
        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(CriarAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Utilizador
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome,
                    EstadoConta = "ativo",
                    DataRegisto = DateTime.Now,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Administrador");
                    _context.Administradores.Add(new Administrador { Id = user.Id });

                    _context.HistoricoAcoesAdmin.Add(new HistoricoAcaoAdmin
                    {
                        AdminId = _userManager.GetUserId(User),
                        AlvoUtilizadorId = user.Id,
                        TipoAcao = "Criação de Administrador",
                        Motivo = "Novo administrador criado via Backoffice",
                        Data = DateTime.Now
                    });

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // =============================================================
        // 4. GESTÃO DE UTILIZADORES (EDITAR / BLOQUEAR / APROVAR)
        // =============================================================
        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null) return NotFound();
            var user = await _context.Users.Include(u => u.Vendedor).Include(u => u.Comprador).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, Utilizador userAtualizado)
        {
            if (id != userAtualizado.Id) return NotFound();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Nome = userAtualizado.Nome;
            user.Email = userAtualizado.Email;
            user.UserName = userAtualizado.Email;

            _context.HistoricoAcoesAdmin.Add(new HistoricoAcaoAdmin
            {
                AdminId = _userManager.GetUserId(User),
                AlvoUtilizadorId = user.Id,
                TipoAcao = "Edição de Perfil",
                Motivo = "Atualização de dados",
                Data = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BloquearUser(string id, string motivo)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            string acao = (user.EstadoConta == "ativo") ? "Bloqueio" : "Desbloqueio";
            user.EstadoConta = (user.EstadoConta == "ativo") ? "bloqueado" : "ativo";

            if (acao == "Desbloqueio") motivo = "Desbloqueio manual";

            _context.HistoricoAcoesAdmin.Add(new HistoricoAcaoAdmin
            {
                AdminId = _userManager.GetUserId(User),
                AlvoUtilizadorId = user.Id,
                TipoAcao = acao,
                Motivo = motivo,
                Data = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarVendedor(string id)
        {
            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor != null)
            {
                vendedor.EstadoAprovacao = "aprovado";
                _context.HistoricoAcoesAdmin.Add(new HistoricoAcaoAdmin
                {
                    AdminId = _userManager.GetUserId(User),
                    AlvoUtilizadorId = id,
                    TipoAcao = "Aprovação de Vendedor",
                    Motivo = "Vendedor validado",
                    Data = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Historico()
        {
            var historico = await _context.HistoricoAcoesAdmin
                .Include(h => h.Admin).ThenInclude(a => a.Utilizador)
                .Include(h => h.AlvoUtilizador)
                .OrderByDescending(h => h.Data)
                .ToListAsync();

            return View(historico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEmailManual(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.EmailConfirmed = true;
                _context.HistoricoAcoesAdmin.Add(new HistoricoAcaoAdmin
                {
                    AdminId = _userManager.GetUserId(User),
                    AlvoUtilizadorId = user.Id,
                    TipoAcao = "Validação Manual de Email",
                    Motivo = "Utilizador não conseguia aceder ao email",
                    Data = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // =============================================================
        // 5. GESTÃO DE MARCAS E MODELOS (ATUALIZADO)
        // =============================================================

        // LISTAR MARCAS
        public async Task<IActionResult> GerirMarcas()
        {
            var marcas = await _context.Marcas
                .Include(m => m.Modelos)
                .OrderBy(m => m.Nome)
                .ToListAsync();
            return View(marcas);
        }

        // CRIAR MARCA (GET)
        public IActionResult CreateMarca()
        {
            return View();
        }

        // CRIAR MARCA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMarca(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                TempData["Error"] = "O nome da marca é obrigatório.";
                return RedirectToAction(nameof(GerirMarcas));
            }

            // Formatação: "audi" -> "Audi"
            string nomeFormatado = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nome.ToLower().Trim());

            if (await _context.Marcas.AnyAsync(m => m.Nome == nomeFormatado))
            {
                TempData["Error"] = $"A marca '{nomeFormatado}' já existe.";
                return RedirectToAction(nameof(GerirMarcas));
            }

            var novaMarca = new Marca { Nome = nomeFormatado };
            _context.Marcas.Add(novaMarca);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Marca criada com sucesso!";
            return RedirectToAction(nameof(GerirMarcas));
        }

        // ADICIONAR MODELO (GET)
        public async Task<IActionResult> CreateModelo(int marcaId)
        {
            var marca = await _context.Marcas.FindAsync(marcaId);
            if (marca == null) return NotFound();

            ViewBag.MarcaNome = marca.Nome;
            ViewBag.MarcaId = marca.Id;
            return View();
        }

        // ADICIONAR MODELO (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModelo(int marcaId, string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                TempData["Error"] = "O nome do modelo é obrigatório.";
                return RedirectToAction(nameof(GerirMarcas));
            }

            // Formatação: "golf gti" -> "Golf Gti"
            string nomeFormatado = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nome.ToLower().Trim());

            // VERIFICAÇÃO DE DUPLICADOS PARA A MESMA MARCA
            var existe = await _context.Modelos.AnyAsync(m => m.MarcaId == marcaId && m.Nome == nomeFormatado);
            if (existe)
            {
                TempData["Error"] = $"O modelo '{nomeFormatado}' já está associado a esta marca.";
                return RedirectToAction(nameof(GerirMarcas));
            }

            var novoModelo = new Modelo { Nome = nomeFormatado, MarcaId = marcaId };
            _context.Modelos.Add(novoModelo);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Modelo adicionado com sucesso!";
            return RedirectToAction(nameof(GerirMarcas));
        }

        // APAGAR MODELO (BACKEND PARA O BOTÃO X)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApagarModelo(int id)
        {
            var modelo = await _context.Modelos.FindAsync(id);
            if (modelo != null)
            {
                _context.Modelos.Remove(modelo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Modelo removido com sucesso.";
            }
            return RedirectToAction(nameof(GerirMarcas));
        }

        // APAGAR MARCA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApagarMarca(int id)
        {
            var marca = await _context.Marcas.Include(m => m.Modelos).FirstOrDefaultAsync(m => m.Id == id);
            if (marca != null)
            {
                _context.Marcas.Remove(marca);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Marca e modelos associados removidos.";
            }
            return RedirectToAction(nameof(GerirMarcas));
        }
    }
}