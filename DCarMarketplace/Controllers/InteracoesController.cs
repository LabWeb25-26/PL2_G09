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
    [Authorize(Roles = "Comprador")] // Segurança: Só Compradores têm acesso
    public class InteracoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizador> _userManager;

        public InteracoesController(ApplicationDbContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =============================================================
        // 1. RESERVAR VEÍCULO (RF-7)
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int anuncioId)
        {
            var user = await _userManager.GetUserAsync(User);
            var anuncio = await _context.Anuncios.FindAsync(anuncioId);

            // Só pode reservar se estiver "ativo"
            if (anuncio == null || anuncio.Estado != "ativo")
            {
                // Em caso de erro, volta ao detalhe (idealmente com mensagem de erro)
                return RedirectToAction("Details", "Anuncios", new { id = anuncioId });
            }

            var reserva = new Reserva
            {
                AnuncioId = anuncioId,
                CompradorId = user.Id,
                Data = DateTime.Now,
                PrazoExpiracao = DateTime.Now.AddDays(3), // Expira em 3 dias
                Estado = "ativa"
            };

            // Atualiza estado do carro para não deixar ninguém mais comprar/reservar
            anuncio.Estado = "reservado";

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Historico));
        }

        // =============================================================
        // 2. MARCAR VISITA (RF-9)
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarVisita(int anuncioId, DateTime dataVisita)
        {
            var user = await _userManager.GetUserAsync(User);

            if (dataVisita <= DateTime.Now)
            {
                return RedirectToAction("Details", "Anuncios", new { id = anuncioId });
            }

            var agenda = new Agenda
            {
                AnuncioId = anuncioId,
                CompradorId = user.Id,
                DataAgenda = DateTime.Now,
                DataVisita = dataVisita,
                Estado = "pendente"
            };

            _context.Agendas.Add(agenda);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Historico));
        }

        // =============================================================
        // 3. CANCELAR RESERVA
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var reserva = await _context.Reservas
                .Include(r => r.Anuncio)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound();

            // Segurança: Só o dono da reserva pode cancelar
            if (reserva.CompradorId != user.Id) return Forbid();

            if (reserva.Estado == "ativa")
            {
                reserva.Estado = "cancelada";

                // Liberta o carro (volta a ficar ativo) se ainda estiver como 'reservado'
                if (reserva.Anuncio.Estado == "reservado")
                {
                    reserva.Anuncio.Estado = "ativo";
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Historico));
        }

        // =============================================================
        // 4. CHECKOUT (PÁGINA DE PAGAMENTO) (RF-11)
        // =============================================================
        [HttpGet]
        public async Task<IActionResult> Checkout(int id)
        {
            var anuncio = await _context.Anuncios
                .Include(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anuncio == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // Verificar se o carro está disponível para este utilizador
            // (Pode comprar se estiver 'ativo' OU se estiver 'reservado' por ele próprio)
            bool isReservadoPorMim = await _context.Reservas
                .AnyAsync(r => r.AnuncioId == id && r.CompradorId == user.Id && r.Estado == "ativa");

            if (anuncio.Estado != "ativo" && !isReservadoPorMim)
            {
                return RedirectToAction("Details", "Anuncios", new { id = id });
            }

            return View(anuncio);
        }

        // =============================================================
        // 5. CONFIRMAR COMPRA (POST) (RF-12)
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCompra(int id, string metodoPagamento)
        {
            var user = await _userManager.GetUserAsync(User);
            var anuncio = await _context.Anuncios.FindAsync(id);

            if (anuncio == null) return NotFound();

            // Criar Registo de Compra
            var compra = new Compra
            {
                AnuncioId = id,
                CompradorId = user.Id,
                Data = DateTime.Now,
                Preco = anuncio.Preco,
                EstadoPagamento = "pago" // Assume sucesso imediato na simulação
            };

            // Atualizar Estado do Anúncio -> VENDIDO
            anuncio.Estado = "vendido";

            // Fechar quaisquer reservas ativas para este carro (já foi vendido)
            var reservasAtivas = await _context.Reservas
                .Where(r => r.AnuncioId == id && r.Estado == "ativa")
                .ToListAsync();

            foreach (var r in reservasAtivas)
            {
                r.Estado = "concluida"; // Marca como terminada porque foi comprado
            }

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Historico));
        }

        // =============================================================
        // 6. HISTÓRICO COMPLETO (RF-8, RF-10, RF-13)
        // =============================================================
        public async Task<IActionResult> Historico()
        {
            var user = await _userManager.GetUserAsync(User);

            // Carregar Reservas
            var reservas = await _context.Reservas
                .Include(r => r.Anuncio).ThenInclude(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Where(r => r.CompradorId == user.Id)
                .OrderByDescending(r => r.Data)
                .ToListAsync();

            // Carregar Visitas
            var visitas = await _context.Agendas
                .Include(a => a.Anuncio).ThenInclude(an => an.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Where(a => a.CompradorId == user.Id)
                .OrderByDescending(a => a.DataVisita)
                .ToListAsync();

            // Carregar Compras
            var compras = await _context.Compras
                .Include(c => c.Anuncio).ThenInclude(a => a.Carro).ThenInclude(c => c.Modelo).ThenInclude(m => m.Marca)
                .Where(c => c.CompradorId == user.Id)
                .OrderByDescending(c => c.Data)
                .ToListAsync();

            var model = new HistoricoCompradorViewModel
            {
                MinhasReservas = reservas,
                MinhasVisitas = visitas,
                MinhasCompras = compras
            };

            return View(model);
        }
    }
}