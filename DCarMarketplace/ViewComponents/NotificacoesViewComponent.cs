using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DCarMarketplace.Data;
using Microsoft.AspNetCore.Identity;
using DCarMarketplace.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DCarMarketplace.ViewComponents
{
    public class NotificacoesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizador> _userManager;

        public NotificacoesViewComponent(ApplicationDbContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated) return Content("");

            var userId = _userManager.GetUserId(UserClaimsPrincipal);

            // Buscar as últimas 5 notificações não lidas
            var notificacoes = await _context.Notificacoes
                .Where(n => n.UtilizadorId == userId && !n.Lida)
                .OrderByDescending(n => n.Data)
                .Take(5)
                .ToListAsync();

            return View(notificacoes);
        }
    }
}