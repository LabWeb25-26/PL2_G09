using DCarMarketplace.Data;
using DCarMarketplace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DCarMarketplace.Controllers
{
    [Authorize]
    public class FavoritosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizador> _userManager;
                                                                                                                                                            
        public FavoritosController(ApplicationDbContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Página de listagem
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var favoritos = await _context.AnunciosFavoritos
                .Where(f => f.UtilizadorId == userId)
                .Include(f => f.Anuncio).ThenInclude(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Include(f => f.Anuncio).ThenInclude(a => a.Fotos)
                .Select(f => f.Anuncio)
                .ToListAsync();

            ViewBag.FavoritosIds = favoritos.Select(a => a.Id).ToList();
            return View(favoritos);
        }

        // Ação de Adicionar/Remover
        [HttpPost]
        public async Task<IActionResult> Toggle(int anuncioId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var fav = await _context.AnunciosFavoritos
                .FirstOrDefaultAsync(f => f.UtilizadorId == userId && f.AnuncioId == anuncioId);

            bool adicionado;
            if (fav != null)
            {
                _context.AnunciosFavoritos.Remove(fav);
                adicionado = false;
            }
            else
            {
                _context.AnunciosFavoritos.Add(new AnuncioFavorito { UtilizadorId = userId, AnuncioId = anuncioId });
                adicionado = true;
            }

            await _context.SaveChangesAsync();
            int total = await _context.AnunciosFavoritos.CountAsync(f => f.UtilizadorId == userId);

            return Json(new { success = true, isFavorited = adicionado, count = total });
        }

        // API para o JavaScript consultar o total
        [HttpGet]
        public async Task<JsonResult> GetTotal()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Json(0);
            int total = await _context.AnunciosFavoritos.CountAsync(f => f.UtilizadorId == userId);
            return Json(total);
        }
    }
}