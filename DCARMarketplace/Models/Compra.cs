using System.ComponentModel.DataAnnotations.Schema;

namespace DCARMarketplace.Models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        public string EstadoPagamento { get; set; } = "pendente";

        public int AnuncioId { get; set; }
        public Anuncio? Anuncio { get; set; }

        public int CompradorId { get; set; }
        public Comprador? Comprador { get; set; }
    }
}