using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DCarMarketplace.Data;
using DCarMarketplace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DCarMarketplace.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Utilizador> _userManager;
        private readonly SignInManager<Utilizador> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<Utilizador> userManager,
            SignInManager<Utilizador> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            // --- NOVO CAMPO: NOME ---
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [Display(Name = "Nome Completo")]
            public string Name { get; set; }

            [Phone]
            [Display(Name = "Telemóvel")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Morada Completa")]
            public string Morada { get; set; }

            [Display(Name = "NIF (Nº Contribuinte)")]
            public string NIF { get; set; }

            [Display(Name = "Tipo de Vendedor")]
            public string Tipo { get; set; }
        }

        public bool IsVendedor { get; set; }
        public bool IsComprador { get; set; }

        private async Task LoadAsync(Utilizador user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Name = user.Nome, // Carrega o nome atual da BD
                PhoneNumber = phoneNumber
            };

            if (await _userManager.IsInRoleAsync(user, "Vendedor"))
            {
                IsVendedor = true;
                var vendedor = await _context.Vendedores.FindAsync(user.Id);
                if (vendedor != null)
                {
                    Input.Morada = vendedor.Morada;
                    Input.NIF = vendedor.NIF;
                    Input.Tipo = vendedor.Tipo;
                }
            }
            else if (await _userManager.IsInRoleAsync(user, "Comprador"))
            {
                IsComprador = true;
                var comprador = await _context.Compradores.FindAsync(user.Id);
                if (comprador != null)
                {
                    Input.Morada = comprador.Morada;
                }
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // 1. Atualizar NOME (Novo)
            if (Input.Name != user.Nome)
            {
                user.Nome = Input.Name;
                await _userManager.UpdateAsync(user);
            }

            // 2. Atualizar TELEFONE
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Erro ao atualizar número de telefone.";
                    return RedirectToPage();
                }
            }

            // 3. Atualizar DADOS ESPECÍFICOS (Vendedor/Comprador)
            if (await _userManager.IsInRoleAsync(user, "Vendedor"))
            {
                var vendedor = await _context.Vendedores.FindAsync(user.Id);
                if (vendedor != null)
                {
                    vendedor.Morada = Input.Morada;
                    vendedor.NIF = Input.NIF;
                    vendedor.Tipo = Input.Tipo;
                    vendedor.Contactos = Input.PhoneNumber;
                    _context.Update(vendedor);
                }
            }
            else if (await _userManager.IsInRoleAsync(user, "Comprador"))
            {
                var comprador = await _context.Compradores.FindAsync(user.Id);
                if (comprador != null)
                {
                    comprador.Morada = Input.Morada;
                    comprador.Contactos = Input.PhoneNumber;
                    _context.Update(comprador);
                }
            }

            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "O seu perfil foi atualizado com sucesso.";
            return RedirectToPage();
        }
    }
}