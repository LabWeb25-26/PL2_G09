using System.ComponentModel.DataAnnotations;

namespace MarketplaceVeiculos.Models
{
    public class Combustivel
    {
        [Key] // Chave Primária
        public int id_combustivel { get; set; }

        public string tipo { get; set; }
    }
}