using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Carro
    {
        [Key]
        public int Id { get; set; }

        // --- DADOS DE IDENTIFICAÇÃO ---
        [StringLength(17)]
        public string? VIN { get; set; } // Opcional

        [Required]
        [StringLength(20)]
        public string Matricula { get; set; }

        // --- DADOS TÉCNICOS GERAIS ---
        [Required]
        public int Ano { get; set; }

        [Required]
        public int Quilometragem { get; set; }

        [Required]
        [StringLength(50)]
        public string Caixa { get; set; } // Manual / Automática

        // --- NOVOS CAMPOS ADICIONADOS ---
        [Required]
        public int NumeroPortas { get; set; } // Ex: 3, 5

        [Required]
        [StringLength(50)]
        public string Cor { get; set; }       // Ex: Preto, Branco

        [Required]
        [StringLength(50)]
        public string Segmento { get; set; }  // Ex: SUV, Sedan

        [Required]
        public int Potencia { get; set; }     // Cavalos (cv)

        [Required]
        public int Cilindrada { get; set; }   // cm3

        // --- CHAVES ESTRANGEIRAS (RELACIONAMENTOS) ---
        [Required]
        public int ModeloId { get; set; }

        [ForeignKey("ModeloId")]
        public virtual Modelo Modelo { get; set; }

        [Required]
        public int CombustivelId { get; set; }

        [ForeignKey("CombustivelId")]
        public virtual Combustivel Combustivel { get; set; }

        // Relação inversa com o Anúncio (Um carro pode ter 1 anúncio)
        public virtual Anuncio? Anuncio { get; set; }
    }
}