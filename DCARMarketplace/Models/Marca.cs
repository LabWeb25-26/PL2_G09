using System.Collections.Generic; // Necessário para usar ICollection

namespace DCARMarketplace.Models
{
    public class Marca
    {
        public int Id { get; set; }

        // Inicializamos como vazio para evitar o erro CS8618
        public string Nome { get; set; } = string.Empty;

        // Inicializamos a lista para não dar erro se tentares adicionar algo logo
        public ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
    }
}