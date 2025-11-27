using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        public int AnuncioId { get; set; }
        public string CompradorId { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;
        public DateTime? PrazoExpiracao { get; set; }
        public string Estado { get; set; } // 'ativa', 'expirada', 'cancelada'

        [ForeignKey("AnuncioId")]
        public virtual Anuncio Anuncio { get; set; }
        [ForeignKey("CompradorId")]
        public virtual Comprador Comprador { get; set; }
    }
}
