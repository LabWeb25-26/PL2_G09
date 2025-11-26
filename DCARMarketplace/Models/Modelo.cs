namespace DCARMarketplace.Models
{
    public class Modelo
    {
        public int Id { get; set; }

        // Inicializamos como vazio
        public string Nome { get; set; } = string.Empty;

        // Chave Estrangeira
        public int MarcaId { get; set; }

        // O "?" (Marca?) diz ao sistema que "pode ser nulo" temporariamente
        // isto resolve o aviso amarelo sem complicar o código
        public Marca? Marca { get; set; }
    }
}