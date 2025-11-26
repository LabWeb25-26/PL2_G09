using Microsoft.AspNetCore.Identity;

namespace Dcar.Models
{
    // Herda de IdentityUser para aproveitar o sistema de login da Microsoft
    public class Utilizador : IdentityUser
    {
        // Podes adicionar campos extra à tabela de login aqui
        public string? NomeCompleto { get; set; }
        public string? FotoPerfil { get; set; }

        // Data de registo, etc.
        public DateTime DataRegisto { get; set; } = DateTime.Now;
    }
}