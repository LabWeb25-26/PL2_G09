using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    [Table("Anuncio")]
    public class Anuncio
    {
        [Key]
        public int id_anuncio { get; set; }

        [Required]
        public string titulo { get; set; }

        public string? descricao { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime? data_fim { get; set; }
        public string? localizacao { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal preco { get; set; }

        public string estado { get; set; } // ativo, reservado, vendido, pausado
        public string? fotos { get; set; } // (Mais tarde isto pode ser uma lista/tabela separada)

        // Chave Estrangeira para Vendedor
        [ForeignKey("Vendedor")]
        public int id_vendedor { get; set; }
        public virtual Vendedor Vendedor { get; set; }

        // Chave Estrangeira para Carro
        // Assumindo que um anúncio tem UM carro
        [ForeignKey("Carro")]
        public int id_carro { get; set; }
        public virtual Carro Carro { get; set; }
    }
}