using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    public class Carro
    {
        [Key]
        public int id_carro { get; set; }

        public string? VIN { get; set; } // O '?' permite que seja nulo (opcional)

        public string matricula { get; set; }

        public int ano { get; set; }

        public int quilometragem { get; set; }

        public string? caixa { get; set; } // O '?' permite que seja nulo (opcional)

        // Chave Estrangeira para Modelo
        [ForeignKey("Modelo")]
        public int id_modelo { get; set; }
        public virtual Modelo Modelo { get; set; }

        // Chave Estrangeira para Combustivel
        [ForeignKey("Combustivel")]
        public int id_combustivel { get; set; }
        public virtual Combustivel Combustivel { get; set; }
    }
}