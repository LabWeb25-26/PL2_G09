using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class FiltroFavorito
    {
        [Key]
        public int Id { get; set; }

        public string UtilizadorId { get; set; } // ID do Comprador

        [ForeignKey("UtilizadorId")]
        public virtual Utilizador Utilizador { get; set; }

        [Required]
        public string Nome { get; set; } // Ex: "BMWs baratos"

        [Required]
        public string UrlQuery { get; set; } // Guardamos o link da pesquisa (ex: "?marca=BMW&precoMax=20000")

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}