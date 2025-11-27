using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DCarMarketplace.Data;
using DCarMarketplace.Models;

namespace DCarMarketplace.Controllers
{
    [Authorize(Roles = "Administrador")] // <--- Só o Admin entra aqui
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizador> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Listagem de Utilizadores
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Vendedor)
                .Include(u => u.Comprador)
                .OrderByDescending(u => u.DataRegisto)
                .ToListAsync();

            return View(users);
        }

        // Aprovar Vendedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprovarVendedor(string id)
        {
            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor != null)
            {
                vendedor.EstadoAprovacao = "aprovado";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Bloquear/Desbloquear Conta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlternarBloqueio(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.EstadoConta = (user.EstadoConta == "ativo") ? "bloqueado" : "ativo";

                // Registo de auditoria (simplificado)
                _context.HistoricoAcoesAdmin.Add(new HistoricoAcaoAdmin
                {
                    AdminId = _userManager.GetUserId(User),
                    AlvoUtilizadorId = user.Id,
                    TipoAcao = user.EstadoConta == "bloqueado" ? "Bloqueio" : "Desbloqueio",
                    Motivo = "Ação via Backoffice",
                    Data = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}