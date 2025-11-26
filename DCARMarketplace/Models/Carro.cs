namespace DCARMarketplace.Models
{
    public class Carro
    {
        public int Id { get; set; }

        // Inicializamos as strings para evitar avisos
        public string VIN { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;

        public int Ano { get; set; }
        public int Quilometragem { get; set; }
        public string Caixa { get; set; } = string.Empty;

        // Relação com Modelo
        public int ModeloId { get; set; }
        public Modelo? Modelo { get; set; }

        // Relação com Combustivel
        public int CombustivelId { get; set; }
        public Combustivel? Combustivel { get; set; }
    }
}