using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Agenda
    {
        [Key]
        public int Id { get; set; }

        public int AnuncioId { get; set; }
        public string CompradorId { get; set; }
                                                                                                                                                                                                                                          
        public DateTime DataAgenda { get; set; } = DateTime.Now; // Quando foi marcado
        public DateTime DataVisita { get; set; } // Para quando é a visita
        public string Estado { get; set; } = "pendente"; // 'pendente', 'realizada', 'cancelada'

        [ForeignKey("AnuncioId")]
        public virtual Anuncio Anuncio { get; set; }
        [ForeignKey("CompradorId")]
        public virtual Comprador Comprador { get; set; }
    }
}
