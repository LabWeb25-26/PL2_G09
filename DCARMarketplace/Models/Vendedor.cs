using System.ComponentModel.DataAnnotations.Schema;

namespace DCARMarketplace.Models
{
    [Table("Vendedor")]
    public class Vendedor : Utilizador
    {
        public string Morada { get; set; } = string.Empty;
        public string Contactos { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // Particular / Empresa
        public string NIF { get; set; } = string.Empty;
        public string EstadoAprovacao { get; set; } = "pendente";
    }
}