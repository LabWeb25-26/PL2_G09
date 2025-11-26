using System.ComponentModel.DataAnnotations.Schema; // Necessário para definir o preço

namespace DCARMarketplace.Models
{
    public class Anuncio
    {
        public int Id { get; set; } // id_anuncio

        // Inicializamos strings para evitar avisos amarelos
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; } // Pode ser nulo (ainda ativo)

        public string Localizacao { get; set; } = string.Empty;

        // Define o formato do dinheiro na base de dados (10 dígitos, 2 decimais)
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        public string Estado { get; set; } = "ativo"; // Valor por defeito
        public string Fotos { get; set; } = string.Empty; // Caminho das imagens

        // Relacionamentos (Chaves Estrangeiras)
        public int CarroId { get; set; }
        public Carro? Carro { get; set; } // ? permite carregar o anúncio sem o objeto Carro obrigatório

        public int VendedorId { get; set; }
        public Utilizador? Vendedor { get; set; } // ? igual ao de cima
    }
}