using Dcar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Dcar.Data; // Importante
using Microsoft.EntityFrameworkCore; // Importante

namespace Dcar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Adicionar o Contexto

        // Injetar a Base de Dados
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Vai buscar apenas os anúncios "Ativos" para mostrar na montra
            var anunciosAtivos = await _context.Anuncios
                .Include(a => a.IdCarroNavigation) // Para saber a marca/modelo
                .Include(a => a.IdCarroNavigation.IdModeloNavigation)
                .Where(a => a.Estado == "ativo") // Filtra só os ativos
                .OrderByDescending(a => a.DataInicio) // Mais recentes primeiro
                .ToListAsync();

            return View(anunciosAtivos);
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