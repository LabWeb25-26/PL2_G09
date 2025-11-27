using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Utilizador : IdentityUser
    {
        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        [Required]
        public string EstadoConta { get; set; } = "ativo"; // 'ativo', 'bloqueado'

        public DateTime DataRegisto { get; set; } = DateTime.Now;

        // Ligações aos Perfis (1 Utilizador pode ter 1 Vendedor e/ou 1 Comprador)
        public virtual Administrador? Administrador { get; set; }
        public virtual Vendedor? Vendedor { get; set; }
        public virtual Comprador? Comprador { get; set; }
    }
}