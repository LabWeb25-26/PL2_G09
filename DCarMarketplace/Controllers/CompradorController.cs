using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DCarMarketplace.Data;
using DCarMarketplace.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DCarMarketplace.Controllers
{
    [Authorize(Roles = "Comprador")] // Só Compradores acedem
    public class CompradorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizador> _userManager;
                                                                                                                                                                                                            
        public CompradorController(ApplicationDbContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =============================================================
        // 1. GESTÃO DE MARCAS FAVORITAS (RF-6)
        // =============================================================

        public async Task<IActionResult> MinhasMarcas()
        {
            var userId = _userManager.GetUserId(User);

            // Carregar todas as marcas
            var todasMarcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();

            // Carregar IDs das marcas que o user já segue
            var marcasSeguidas = await _context.MarcasFavoritas
                .Where(mf => mf.UtilizadorId == userId)
                .Select(mf => mf.MarcaId)
                .ToListAsync();

            ViewBag.MarcasSeguidas = marcasSeguidas;
            return View(todasMarcas);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleMarca(int id)
        {
            var userId = _userManager.GetUserId(User);
            var marcaFav = await _context.MarcasFavoritas.FindAsync(userId, id);

            bool seguindo;
            if (marcaFav != null)
            {
                _context.MarcasFavoritas.Remove(marcaFav);
                seguindo = false;
            }
            else
            {
                _context.MarcasFavoritas.Add(new MarcaFavorita { UtilizadorId = userId, MarcaId = id });
                seguindo = true;
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, seguindo = seguindo });
        }

        // =============================================================
        // 2. GESTÃO DE FILTROS FAVORITOS (RF-5)
        // =============================================================

        public async Task<IActionResult> MeusFiltros()
        {
            var userId = _userManager.GetUserId(User);
            var filtros = await _context.FiltrosFavoritos
                .Where(f => f.UtilizadorId == userId)
                .OrderByDescending(f => f.DataCriacao)
                .ToListAsync();

            return View(filtros);
        }

        [HttpPost]
        public async Task<IActionResult> SalvarFiltro(string nome, string queryUrl)
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(queryUrl))
                return BadRequest();

            var userId = _userManager.GetUserId(User);

            var filtro = new FiltroFavorito
            {
                UtilizadorId = userId,
                Nome = nome,
                UrlQuery = queryUrl,
                DataCriacao = DateTime.Now
            };

            _context.FiltrosFavoritos.Add(filtro);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ApagarFiltro(int id)
        {
            var userId = _userManager.GetUserId(User);
            var filtro = await _context.FiltrosFavoritos.FirstOrDefaultAsync(f => f.Id == id && f.UtilizadorId == userId);

            if (filtro != null)
            {
                _context.FiltrosFavoritos.Remove(filtro);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MeusFiltros));
        }
    }
}