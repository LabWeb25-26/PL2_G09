using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class HistoricoAcaoAdmin
    {
        [Key]
        public int Id { get; set; }

        // --- QUEM FEZ A AÇÃO (ADMIN) ---
        public string AdminId { get; set; }

        [ForeignKey("AdminId")]
        // Voltamos a usar 'Administrador' para corrigir o erro de compilação
        public virtual Administrador Admin { get; set; }

        // --- ALVO (Pode ser um Utilizador OU um Anúncio) ---
        public string? AlvoUtilizadorId { get; set; }

        [ForeignKey("AlvoUtilizadorId")]
        public virtual Utilizador? AlvoUtilizador { get; set; }

        public int? AlvoAnuncioId { get; set; }

        [ForeignKey("AlvoAnuncioId")]
        public virtual Anuncio? AlvoAnuncio { get; set; }

        // --- DETALHES ---
        public DateTime Data { get; set; } = DateTime.Now;
        public string TipoAcao { get; set; }
        public string Motivo { get; set; }
    }
}