using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    public class Agenda
    {
        [Key]
        public int id_agenda { get; set; }

        [ForeignKey("Comprador")]
        public int id_comprador { get; set; }

        [ForeignKey("Anuncio")]
        public int id_anuncio { get; set; }

        public DateTime data_agenda { get; set; } // Data em que foi marcada
        public DateTime data_visita { get; set; } // Data da visita
        public string? estado { get; set; } = "pendente";

        // Propriedades de navegação
        public virtual Comprador Comprador { get; set; }
        public virtual Anuncio Anuncio { get; set; }
    }
}