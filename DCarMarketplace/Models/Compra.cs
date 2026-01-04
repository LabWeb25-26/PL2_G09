using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        public int AnuncioId { get; set; }
        public string CompradorId { get; set; }
                                                                    
        public DateTime Data { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        public string EstadoPagamento { get; set; } // 'pendente', 'pago'

        [ForeignKey("AnuncioId")]
        public virtual Anuncio Anuncio { get; set; }
        [ForeignKey("CompradorId")]
        public virtual Comprador Comprador { get; set; }
    }
}
