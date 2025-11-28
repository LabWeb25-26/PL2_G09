using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DCarMarketplace.Models.ViewModels
{
    public class EditarAnuncioViewModel
    {
        public int Id { get; set; }

        // --- ANÚNCIO ---
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, MinimumLength = 5)]
        public string Titulo { get; set; }

        [Required]
        [MinLength(30)]
        public string Descricao { get; set; }

        [Required]
        [Range(100, 5000000)]
        public decimal Preco { get; set; }

        [Required]
        public string Localizacao { get; set; }

        // --- CARRO (DADOS TÉCNICOS) ---
        [Required]
        public string Matricula { get; set; }

        public string? VIN { get; set; }

        [Required]
        public int Ano { get; set; }

        [Required]
        public int Quilometragem { get; set; }

        public string Caixa { get; set; }

        [Required]
        public string Cor { get; set; }

        [Required]
        public int NumeroPortas { get; set; }

        [Required]
        public string Segmento { get; set; }

        [Required]
        public int Potencia { get; set; }

        [Required]
        public int Cilindrada { get; set; }

        // --- CHAVES ---
        public int MarcaId { get; set; }
        public int ModeloId { get; set; }
        public int CombustivelId { get; set; }

        // --- FOTOS ---
        public string? FotosAtuais { get; set; }

        [Display(Name = "Adicionar Novas Fotos")]
        public List<IFormFile>? NovasFotos { get; set; }
    }
}