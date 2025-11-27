using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using DCarMarketplace.Models; // O teu namespace
using DCarMarketplace.Data;   // O namespace do contexto
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DCarMarketplace.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Utilizador> _signInManager;
        private readonly UserManager<Utilizador> _userManager;
        private readonly IUserStore<Utilizador> _userStore;
        private readonly IUserEmailStore<Utilizador> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<Utilizador> userManager,
            IUserStore<Utilizador> userStore,
            SignInManager<Utilizador> signInManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "A password e a confirmação não coincidem.")]
            public string ConfirmPassword { get; set; }

            // --- CAMPOS PERSONALIZADOS ---
            [Required]
            [Display(Name = "Nome Completo")]
            public string Nome { get; set; }

            [Required]
            [Display(Name = "Tipo de Conta")]
            public string TipoConta { get; set; } // "Comprador" ou "Vendedor"

            [Display(Name = "NIF (Apenas Vendedores)")]
            public string? NIF { get; set; }

            [Display(Name = "Morada")]
            public string? Morada { get; set; }

            [Display(Name = "Telemóvel")]
            public string? Contactos { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.Nome = Input.Nome;
                user.DataRegisto = DateTime.Now;
                user.EstadoConta = "ativo";

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Nova conta criada.");

                    if (Input.TipoConta == "Vendedor")
                    {
                        var vendedor = new Vendedor
                        {
                            Id = user.Id,
                            NIF = Input.NIF ?? "N/A",
                            Morada = Input.Morada,
                            Contactos = Input.Contactos,
                            Tipo = "Particular",
                            EstadoAprovacao = "pendente"
                        };
                        _context.Vendedores.Add(vendedor);
                        await _userManager.AddToRoleAsync(user, "Vendedor");
                    }
                    else
                    {
                        var comprador = new Comprador
                        {
                            Id = user.Id,
                            Morada = Input.Morada,
                            Contactos = Input.Contactos
                        };
                        _context.Compradores.Add(comprador);
                        await _userManager.AddToRoleAsync(user, "Comprador");
                    }

                    await _context.SaveChangesAsync();
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }

        private Utilizador CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Utilizador>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Utilizador)}'.");
            }
        }

        private IUserEmailStore<Utilizador> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Utilizador>)_userStore;
        }
    }
}