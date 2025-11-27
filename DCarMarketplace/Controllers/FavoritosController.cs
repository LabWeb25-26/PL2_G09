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

        // 1. API: Adicionar/Remover Favorito (Chamado pelo Coração)
        [HttpPost]
        public async Task<IActionResult> Toggle(int anuncioId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var favoritoExistente = await _context.AnunciosFavoritos
                .FirstOrDefaultAsync(f => f.UtilizadorId == user.Id && f.AnuncioId == anuncioId);

            bool adicionado;

            if (favoritoExistente != null)
            {
                _context.AnunciosFavoritos.Remove(favoritoExistente);
                adicionado = false;
            }
            else
            {
                var novoFavorito = new AnuncioFavorito { UtilizadorId = user.Id, AnuncioId = anuncioId };
                _context.AnunciosFavoritos.Add(novoFavorito);
                adicionado = true;
            }

            await _context.SaveChangesAsync();

            // Conta quantos favoritos o user tem agora para atualizar o menu
            int total = await _context.AnunciosFavoritos.CountAsync(f => f.UtilizadorId == user.Id);

            return Json(new { success = true, isFavorited = adicionado, count = total });
        }

        // 2. Página: Meus Favoritos
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var favoritos = await _context.AnunciosFavoritos
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a.Carro)
                        .ThenInclude(c => c.Modelo)
                            .ThenInclude(m => m.Marca)
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a.Carro)
                        .ThenInclude(c => c.Combustivel)
                .Include(f => f.Anuncio)
                    .ThenInclude(a => a.Vendedor)
                        .ThenInclude(v => v.Utilizador)
                .Where(f => f.UtilizadorId == user.Id)
                .Select(f => f.Anuncio) // Extraímos apenas os anúncios para reutilizar a View
                .ToListAsync();

            ViewData["Title"] = "Meus Favoritos";
            // Reutilizamos a view de listagem de anúncios, mas passamos a lista de favoritos
            return View("~/Views/Anuncios/Index.cshtml", favoritos);
        }

        // 3. API: Obter número total (para atualizar o menu ao carregar a página)
        [HttpGet]
        public async Task<JsonResult> GetTotal()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(0);

            int total = await _context.AnunciosFavoritos.CountAsync(f => f.UtilizadorId == user.Id);
            return Json(total);
        }
    }
}