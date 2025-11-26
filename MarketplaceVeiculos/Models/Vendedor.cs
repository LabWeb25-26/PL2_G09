using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    // Esta classe "herda" tudo do Utilizador
    [Table("Vendedor")] // Diz ao EF para mapear para a tabela Vendedor
    public class Vendedor : Utilizador
    {
        public string? morada { get; set; }
        public string? contactos { get; set; }
        public string? tipo { get; set; } // Particular/Empresa
        public string? NIF { get; set; }
        public string? estado_aprovacao { get; set; } = "pendente";
    }
}