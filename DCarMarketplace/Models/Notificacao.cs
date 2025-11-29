using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Notificacao
    {
        [Key]
        public int Id { get; set; }

        public string UtilizadorId { get; set; } // Quem recebe a notificação

        [ForeignKey("UtilizadorId")]
        public virtual Utilizador Utilizador { get; set; }

        public string Mensagem { get; set; }

        public string? Link { get; set; } // Link para o carro novo

        public bool Lida { get; set; } = false;

        public DateTime Data { get; set; } = DateTime.Now;
    }
}