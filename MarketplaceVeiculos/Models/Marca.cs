using System.ComponentModel.DataAnnotations;

namespace MarketplaceVeiculos.Models
{
    public class Marca
    {
        [Key] // Isto indica que é a Chave Primária
        public int id_marca { get; set; }

        public string nome_marca { get; set; }
    }
}