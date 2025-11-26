namespace DCARMarketplace.Models
{
    public class Agenda
    {
        public int Id { get; set; }
        public DateTime DataAgenda { get; set; }
        public DateTime DataVisita { get; set; }
        public string Estado { get; set; } = "pendente";

        public int AnuncioId { get; set; }
        public Anuncio? Anuncio { get; set; }

        public int CompradorId { get; set; }
        public Comprador? Comprador { get; set; }
    }
}