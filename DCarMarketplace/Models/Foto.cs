using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Foto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FicheiroNome { get; set; }

        // Chave Estrangeira para o Anúncio
        public int AnuncioId { get; set; }

        [ForeignKey("AnuncioId")]
        public virtual Anuncio Anuncio { get; set; }
    }
}