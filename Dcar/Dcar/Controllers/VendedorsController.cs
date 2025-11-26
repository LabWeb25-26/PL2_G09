using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dcar.Data;
using Dcar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Dcar.Controllers
{
    [Authorize]
    public class VendedorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // MUDANÇA AQUI: Agora usamos <Utilizador> em vez de <IdentityUser>
        private readonly UserManager<Utilizador> _userManager;

        // MUDANÇA AQUI NO CONSTRUTOR TAMBÉM:
        public VendedorsController(ApplicationDbContext context, UserManager<Utilizador> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Vendedors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vendedors.ToListAsync());
        }

        // GET: Vendedors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedors
                .FirstOrDefaultAsync(m => m.IdVendedor == id);
            if (vendedor == null)
            {
                return NotFound();
            }

            return View(vendedor);
        }

        // GET: Vendedors/Create
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            if (_context.Vendedors.Any(v => v.UserId == userId))
            {
                // return RedirectToAction(nameof(Index)); 
            }
            return View();
        }

        // POST: Vendedors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVendedor,Nome,Morada,Contactos,Tipo,Nif")] Vendedor vendedor)
        {
            // MUDANÇA AQUI: O GetUserId funciona igual, mas agora está ligado ao Utilizador correto
            vendedor.UserId = _userManager.GetUserId(User);
            vendedor.EstadoAprovacao = "pendente";

            ModelState.Remove("UserId");
            ModelState.Remove("EstadoAprovacao");
            ModelState.Remove("Anuncios");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(vendedor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao gravar: " + ex.Message);
                }
            }
            return View(vendedor);
        }

        // GET: Vendedors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedors.FindAsync(id);
            if (vendedor == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (vendedor.UserId != userId)
            {
                return Unauthorized();
            }

            return View(vendedor);
        }

        // POST: Vendedors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVendedor,UserId,Nome,Morada,Contactos,Tipo,Nif,EstadoAprovacao")] Vendedor vendedor)
        {
            if (id != vendedor.IdVendedor)
            {
                return NotFound();
            }

            ModelState.Remove("Anuncios");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendedorExists(vendedor.IdVendedor))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vendedor);
        }

        // GET: Vendedors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedors
                .FirstOrDefaultAsync(m => m.IdVendedor == id);
            if (vendedor == null)
            {
                return NotFound();
            }

            return View(vendedor);
        }

        // POST: Vendedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendedor = await _context.Vendedors.FindAsync(id);
            if (vendedor != null)
            {
                _context.Vendedors.Remove(vendedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendedorExists(int id)
        {
            return _context.Vendedors.Any(e => e.IdVendedor == id);
        }
    }
}