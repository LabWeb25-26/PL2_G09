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
        // 1. DASHBOARD (ESTATÍSTICAS GERAIS - RF-26)
        // =============================================================
        public async Task<IActionResult> Dashboard()
        {
            // 1. Contagens de Utilizadores
            int totalUsers = await _context.Users.CountAsync();
            int totalVendedores = await _context.Vendedores.CountAsync();
            int totalCompradores = await _context.Compradores.CountAsync();

            // 2. Contagens de Anúncios
            int ativos = await _context.Anuncios.CountAsync(a => a.Estado == "ativo");
            int vendidos = await _context.Anuncios.CountAsync(a => a.Estado == "vendido");

            // 3. Dados Financeiros (Tabela Compras)
            int numVendas = await _context.Compras.CountAsync();
            decimal valorTotal = 0;
            if (numVendas > 0)
            {
                valorTotal = await _context.Compras.SumAsync(c => c.Preco);
            }

            // 4. Top 5 Marcas (Baseado em anúncios ativos, já que ainda temos poucas vendas)
            var topMarcas = await _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .GroupBy(a => a.Carro.Modelo.Marca.Nome)
                .Select(g => new { Marca = g.Key, Quantidade = g.Count() })
                .OrderByDescending(x => x.Quantidade)
                .Take(5)
                .ToDictionaryAsync(x => x.Marca, x => x.Quantidade);

            // Preencher ViewModel
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
        // 2. LISTA DE UTILIZADORES (Mantém-se igual)
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

                    // Auditoria
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
        // 4. EDITAR / BLOQUEAR / APROVAR
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

            string acao = "";
            if (user.EstadoConta == "ativo")
            {
                user.EstadoConta = "bloqueado";
                acao = "Bloqueio";
            }
            else
            {
                user.EstadoConta = "ativo";
                acao = "Desbloqueio";
                motivo = "Desbloqueio manual";
            }

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
                .Include(h => h.Admin).ThenInclude(a => a.Utilizador) // Para ver o nome do Admin
                .Include(h => h.AlvoUtilizador) // Para ver quem foi bloqueado
                .OrderByDescending(h => h.Data)
                .ToListAsync();

            return View(historico);
        }
    }
}