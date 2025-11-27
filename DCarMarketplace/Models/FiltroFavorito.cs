using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class FiltroFavorito
    {
        [Key]
        public int Id { get; set; }

        public string CompradorId { get; set; }
        public string NomeFiltro { get; set; }
        public string Criterios { get; set; } // JSON

        [ForeignKey("CompradorId")]
        public virtual Comprador Comprador { get; set; }
    }
}
