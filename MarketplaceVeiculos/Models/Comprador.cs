using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    [Table("Comprador")]
    public class Comprador : Utilizador
    {
        public string? morada { get; set; }
        public string? contactos { get; set; }
    }
}