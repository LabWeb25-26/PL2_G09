using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    public class Modelo
    {
        [Key]
        public int id_modelo { get; set; }

        public string nome_modelo { get; set; }

        // Chave Estrangeira para a Marca
        [ForeignKey("Marca")] // "Marca" é o nome da propriedade de navegação abaixo
        public int id_marca { get; set; }
        public virtual Marca Marca { get; set; } // Propriedade de navegação
    }
}