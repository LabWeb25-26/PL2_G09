using DCarMarketplace.Data;
using DCarMarketplace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DCarMarketplace.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. CARREGAR MARCAS (CRÍTICO)
            // Usamos 'ListaMarcas' para não haver confusão com outros nomes
            ViewBag.ListaMarcas = await _context.Marcas.OrderBy(m => m.Nome).ToListAsync();

            // 2. CARREGAR DESTAQUES
            var destaques = await _context.Anuncios
                .Include(a => a.Carro)
                    .ThenInclude(c => c.Combustivel)
                .Include(a => a.Vendedor)
                    .ThenInclude(v => v.Utilizador)
                .Where(a => a.Estado == "ativo")
                .OrderByDescending(a => a.DataInicio)
                .Take(6)
                .ToListAsync();

            return View(destaques);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}