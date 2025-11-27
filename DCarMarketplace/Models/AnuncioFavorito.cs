using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class AnuncioFavorito
    {
        [Key]
        public int Id { get; set; }

        public string UtilizadorId { get; set; }
        public int AnuncioId { get; set; }

        [ForeignKey("UtilizadorId")]
        public virtual Utilizador Utilizador { get; set; }

        [ForeignKey("AnuncioId")]
        public virtual Anuncio Anuncio { get; set; }
    }
}