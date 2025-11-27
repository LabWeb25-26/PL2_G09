using System.ComponentModel.DataAnnotations;

namespace DCarMarketplace.Models.ViewModels
{
    public class CriarAnuncioViewModel
    {
        // --- DADOS DO ANÚNCIO ---
        [Required(ErrorMessage = "O título é obrigatório")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Descricao { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser positivo")]
        public decimal Preco { get; set; }

        public string Localizacao { get; set; }

        // --- DADOS DO CARRO ---
        [Required(ErrorMessage = "A matrícula é obrigatória")]
        public string Matricula { get; set; }

        public int Ano { get; set; }

        [Required]
        public int Quilometragem { get; set; }

        public string Caixa { get; set; } // Manual ou Automática

        // Dropdowns (Chaves Estrangeiras)
        [Required(ErrorMessage = "Selecione uma marca")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "Selecione um combustível")]
        public int CombustivelId { get; set; }

        // Nota: O Modelo terá de ser tratado (simplificação: vamos assumir que selecionas a Marca e depois tratamos o modelo)
        // Para já, para não complicar com AJAX, vamos assumir um modelo genérico ou pedir para escrever o nome do modelo se preferires. 
        // Mas como a tua BD exige ModeloId, vamos precisar de uma dropdown de Modelos.
        [Required]
        public int ModeloId { get; set; }
    }
}