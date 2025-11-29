using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class MarcaFavorita
    {
        // Chave composta configurada no DbContext
        public string UtilizadorId { get; set; }
        [ForeignKey("UtilizadorId")]
        public virtual Utilizador Utilizador { get; set; }

        public int MarcaId { get; set; }
        [ForeignKey("MarcaId")]
        public virtual Marca Marca { get; set; }
    }
}