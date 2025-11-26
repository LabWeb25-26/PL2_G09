using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    public class Reserva
    {
        // Chave Primária Composta (parte 1) e Chave Estrangeira
        [ForeignKey("Anuncio")]
        public int id_anuncio { get; set; }

        // Chave Primária Composta (parte 2) e Chave Estrangeira
        [ForeignKey("Comprador")]
        public int id_comprador { get; set; }

        public DateTime data { get; set; }
        public string estado { get; set; }
        public DateTime? prazo_expiracao { get; set; }

        // Propriedades de navegação
        public virtual Anuncio Anuncio { get; set; }
        public virtual Comprador Comprador { get; set; }
    }
}