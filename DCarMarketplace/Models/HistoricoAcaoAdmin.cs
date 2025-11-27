using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class HistoricoAcaoAdmin
    {
        [Key]
        public int Id { get; set; }

        public string AdminId { get; set; }
        public string? AlvoUtilizadorId { get; set; }
        public int? AlvoAnuncioId { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;
        public string TipoAcao { get; set; } // 'bloqueio_user', 'pausa_anuncio', 'aprovacao_vendedor'
        public string Motivo { get; set; }

        [ForeignKey("AdminId")]
        public virtual Administrador Admin { get; set; }

        [ForeignKey("AlvoUtilizadorId")]
        public virtual Utilizador? AlvoUtilizador { get; set; }

        [ForeignKey("AlvoAnuncioId")]
        public virtual Anuncio? AlvoAnuncio { get; set; }
    }
}
