using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class MarcaFavorita
    {
        // Chave composta deve ser definida no DbContext
        public string CompradorId { get; set; }
        public int MarcaId { get; set; }

        [ForeignKey("CompradorId")]
        public virtual Comprador Comprador { get; set; }
        [ForeignKey("MarcaId")]
        public virtual Marca Marca { get; set; }
    }
}