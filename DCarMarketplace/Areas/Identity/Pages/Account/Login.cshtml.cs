using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DCarMarketplace.Models; // Importante para o Utilizador
using DCarMarketplace.Data;   // Importante para o DbContext
using Microsoft.EntityFrameworkCore;

namespace DCarMarketplace.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<Utilizador> _userManager;
        private readonly SignInManager<Utilizador> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context; // Adicionado para ler o motivo

        public LoginModel(SignInManager<Utilizador> signInManager,
            ILogger<LoginModel> logger,
            UserManager<Utilizador> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Lembrar de mim?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // 1. Obter o utilizador pelo email
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user != null)
                {
                    // 2. VERIFICAR SE ESTÁ BLOQUEADO (Requisito: Motivo do bloqueio)
                    if (user.EstadoConta == "bloqueado")
                    {
                        // Vai buscar o último registo de bloqueio ao histórico
                        var ultimoBloqueio = await _context.HistoricoAcoesAdmin
                            .Where(h => h.AlvoUtilizadorId == user.Id && h.TipoAcao == "Bloqueio")
                            .OrderByDescending(h => h.Data)
                            .FirstOrDefaultAsync();

                        string motivo = ultimoBloqueio?.Motivo ?? "Violação dos termos de serviço.";

                        ModelState.AddModelError(string.Empty, $"A sua conta está BLOQUEADA. Motivo: {motivo}");
                        return Page();
                    }

                    // 3. VERIFICAR SE O EMAIL ESTÁ CONFIRMADO (Requisito: Validação de Email)
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "Tem de confirmar o seu email antes de entrar. Verifique a sua caixa de correio.");
                        return Page();
                    }
                }

                // 4. Tentar Login Normal
                // O lockoutOnFailure a true ativa o bloqueio automático após X tentativas falhadas
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tentativa de login inválida.");
                    return Page();
                }
            }

            // Se chegámos aqui, algo falhou, volta a mostrar o formulário
            return Page();
        }
    }
}