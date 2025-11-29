using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DCarMarketplace.Data;
using System.Threading.Tasks;

namespace DCarMarketplace.Controllers
{
    [Authorize]
    public class NotificacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            var notificacao = await _context.Notificacoes.FindAsync(id);
            if (notificacao != null)
            {
                notificacao.Lida = true;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}