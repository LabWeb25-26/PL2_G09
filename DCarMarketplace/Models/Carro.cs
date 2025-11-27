using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Carro
    {
        [Key]
        public int Id { get; set; }

        public string? VIN { get; set; }

        [Required]
        [StringLength(20)]
        public string Matricula { get; set; }

        public int Ano { get; set; }
        public int Quilometragem { get; set; }
        public string Caixa { get; set; } // Manual/Automática

        // Chaves Estrangeiras
        public int ModeloId { get; set; }
        public int CombustivelId { get; set; }

        [ForeignKey("ModeloId")]
        public virtual Modelo Modelo { get; set; }

        [ForeignKey("CombustivelId")]
        public virtual Combustivel Combustivel { get; set; }

        // Um carro pode estar num anúncio (1-para-0..1)
        public virtual Anuncio? Anuncio { get; set; }
    }
}
