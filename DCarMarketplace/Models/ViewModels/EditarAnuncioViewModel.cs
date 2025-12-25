using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DCarMarketplace.Models; // Necessário para reconhecer a classe Foto

namespace DCarMarketplace.Models.ViewModels
{
    public class EditarAnuncioViewModel
    {
        public int Id { get; set; }

        // --- ANÚNCIO ---
        [Display(Name = "Título do Anúncio")]
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "O título deve ter entre 5 e 100 caracteres.")]
        public string Titulo { get; set; }

        [Display(Name = "Descrição Detalhada")]
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [MinLength(30, ErrorMessage = "A descrição deve ser mais detalhada (mínimo 30 caracteres).")]
        public string Descricao { get; set; }

        [Display(Name = "Preço (€)")]
        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(1, 10000000, ErrorMessage = "O preço deve ser um valor positivo válido.")]
        public decimal Preco { get; set; }

        [Display(Name = "Localização")]
        [Required(ErrorMessage = "A localização é obrigatória (ex: Lisboa,Portugal).")]
        public string Localizacao { get; set; }

        // --- CARRO (DADOS TÉCNICOS) ---
        [Display(Name = "Matrícula")]
        [Required(ErrorMessage = "A matrícula é obrigatória.")]
        [RegularExpression(@"^(([A-Za-z]{2}-\d{2}-\d{2})|(\d{2}-\d{2}-[A-Za-z]{2})|(\d{2}-[A-Za-z]{2}-\d{2})|([A-Za-z]{2}-\d{2}-[A-Za-z]{2}))$",
            ErrorMessage = "Formato inválido. Ex: AA-00-00, 00-00-AA, 00-AA-00 ou AA-00-AA")]
        public string Matricula { get; set; }

        [Display(Name = "VIN (Nº de Quadro)")]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "O VIN deve ter exatamente 17 caracteres.")]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "VIN inválido. Não pode conter as letras I, O ou Q e deve ter 17 caracteres.")]
        public string? VIN { get; set; }

        [Display(Name = "Ano")]
        [Required(ErrorMessage = "O ano é obrigatório.")]
        public int Ano { get; set; }

        [Display(Name = "Quilometragem")]
        [Required(ErrorMessage = "A quilometragem é obrigatória.")]
        [Range(0, 2000000, ErrorMessage = "A quilometragem não pode ser negativa.")]
        public int Quilometragem { get; set; }

        [Required(ErrorMessage = "Selecione o tipo de caixa de velocidades.")]
        public string Caixa { get; set; }

        [Display(Name = "Cor")]
        [Required(ErrorMessage = "A cor é obrigatória.")]
        public string Cor { get; set; }

        [Display(Name = "Nº de Portas")]
        [Required(ErrorMessage = "Indique o número de portas.")]
        public int NumeroPortas { get; set; }

        [Display(Name = "Segmento (Categoria)")]
        [Required(ErrorMessage = "O segmento é obrigatório.")]
        public string Segmento { get; set; }

        [Display(Name = "Potência (cv)")]
        [Required(ErrorMessage = "A potência é obrigatória.")]
        [Range(1, 5000, ErrorMessage = "Insira uma potência válida (valor positivo).")]
        public int Potencia { get; set; }

        [Display(Name = "Cilindrada (cm³)")]
        [Required(ErrorMessage = "A cilindrada é obrigatória.")]
        [Range(50, 20000, ErrorMessage = "Insira uma cilindrada válida (valor positivo).")]
        public int Cilindrada { get; set; }

        // --- CHAVES (Foreign Keys) ---
        [Required(ErrorMessage = "Selecione a marca.")]
        [Range(1, int.MaxValue, ErrorMessage = "Marca inválida.")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "Selecione o modelo.")]
        [Range(1, int.MaxValue, ErrorMessage = "Modelo inválido.")]
        public int ModeloId { get; set; }

        [Required(ErrorMessage = "Selecione o tipo de combustível.")]
        [Range(1, int.MaxValue, ErrorMessage = "Combustível inválido.")]
        public int CombustivelId { get; set; }

        // --- FOTOS ---

        // Lista das fotos que já estão guardadas na base de dados
        public List<Foto> FotosAtuais { get; set; } = new List<Foto>();

        // Ficheiros de imagem carregados no momento da edição
        [Display(Name = "Adicionar Novas Fotos")]
        public List<IFormFile>? NovasFotos { get; set; }
    }
}