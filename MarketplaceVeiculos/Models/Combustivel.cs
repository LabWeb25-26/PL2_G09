using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    [Table("Combustivel")]
    public class Combustivel
    {
        [Key] // Chave Primária
        public int id_combustivel { get; set; }

        public string tipo { get; set; }
    }
}