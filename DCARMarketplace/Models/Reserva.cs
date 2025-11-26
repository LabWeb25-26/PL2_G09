namespace DCARMarketplace.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public DateTime? PrazoExpiracao { get; set; }
        public string Estado { get; set; } = "ativa";

        public int AnuncioId { get; set; }
        public Anuncio? Anuncio { get; set; }

        public int CompradorId { get; set; }
        public Comprador? Comprador { get; set; }
    }
}