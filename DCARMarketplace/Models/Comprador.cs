using System.ComponentModel.DataAnnotations.Schema;

namespace DCARMarketplace.Models
{
    [Table("Comprador")]
    public class Comprador : Utilizador
    {
        public string Morada { get; set; } = string.Empty;
        public string Contactos { get; set; } = string.Empty;
    }
}