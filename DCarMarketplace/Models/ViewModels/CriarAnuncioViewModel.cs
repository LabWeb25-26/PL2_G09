using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DCarMarketplace.Models.ViewModels
{
    public class CriarAnuncioViewModel
    {
        // --- DADOS DO ANÚNCIO ---
        [Required(ErrorMessage = "O título é obrigatório")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [MinLength(30, ErrorMessage = "A descrição deve ter pelo menos 30 caracteres.")] // Correção Descrição
        public string Descricao { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "O preço não pode ser negativo.")] // Correção Negativos
        public decimal Preco { get; set; }

        public string Localizacao { get; set; }

        // --- DADOS DO CARRO ---
        [Required(ErrorMessage = "A matrícula é obrigatória")]
        // Regex: Garante que existem pelo menos 2 letras em qualquer posição (Maiúsculas ou Minúsculas)
        [RegularExpression(@".*[a-zA-Z].*[a-zA-Z].*", ErrorMessage = "A matrícula deve conter pelo menos 2 letras.")]
        public string Matricula { get; set; }

        [Required]
        // O Range valida se o ano é válido. A lista dropdown ajuda, mas isto protege o servidor.
        [Range(1900, 2026, ErrorMessage = "Ano inválido.")]
        public int Ano { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Os quilómetros não podem ser negativos.")] // Correção Negativos
        public int Quilometragem { get; set; }

        public string Caixa { get; set; }

        [Required(ErrorMessage = "Selecione uma marca")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "Selecione um combustível")]
        public int CombustivelId { get; set; }

        [Required]
        public int ModeloId { get; set; }

        [Display(Name = "Fotografias (Máx 9)")]
        public List<IFormFile> ImagensFicheiros { get; set; }
    }
}