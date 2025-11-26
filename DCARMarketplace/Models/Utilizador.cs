namespace DCARMarketplace.Models
{
    public class Utilizador
    {
        public int Id { get; set; } // id_utilizador

        // Inicializamos como vazio para evitar avisos (CS8618)
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Definimos o padrão como "ativo", conforme o teu relatório
        public string EstadoConta { get; set; } = "ativo";
    }
}