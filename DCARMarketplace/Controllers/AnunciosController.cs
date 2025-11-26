using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DCARMarketplace.Data;
using DCARMarketplace.Models;

namespace DCARMarketplace.Controllers
{
    public class AnunciosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnunciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Anuncios (Lista de Carros)
        public async Task<IActionResult> Index()
        {
            var anuncios = await _context.Anuncios
                .Include(a => a.Carro)
                .ThenInclude(c => c!.Modelo)
                .ThenInclude(m => m!.Marca)
                .ToListAsync();

            return View(anuncios);
        }

        // --- NOVO CÓDIGO AQUI EM BAIXO ---

        // GET: Anuncios/Details/5 (Detalhes de um Carro)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.Vendedor)       // Carrega o Vendedor
                .Include(a => a.Carro)
                .ThenInclude(c => c!.Modelo)    // Carrega Modelo
                .ThenInclude(m => m!.Marca)     // Carrega Marca
                .Include(a => a.Carro)
                .ThenInclude(c => c!.Combustivel) // Carrega Combustível (Importante!)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (anuncio == null)
            {
                return NotFound();
            }

            return View(anuncio);
        }
    }
}