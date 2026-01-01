using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using DCarMarketplace.Models;

namespace DCarMarketplace.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<Utilizador> _userManager;

        public RegisterConfirmationModel(UserManager<Utilizador> userManager)
        {
            _userManager = userManager;
        }

        public string Email { get; set; }
        public bool DisplayConfirmAccountLink { get; set; }
        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            // --- MUDANÇA CRUCIAL AQUI ---
            // Definimos como 'false' para que o link NÃO apareça na página web.
            // Isto obriga o utilizador a ir procurar o link ao email.
            DisplayConfirmAccountLink = false;

            // Se DisplayConfirmAccountLink for false, não precisamos de gerar o link aqui,
            // pois o link real já foi enviado pelo EmailSender no momento do registo.

            return Page();
        }
    }
}