using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    public class Compra
    {
        // Chave Primária Composta (parte 1) e Chave Estrangeira
        [ForeignKey("Comprador")]
        public int id_comprador { get; set; }

        // Chave Primária Composta (parte 2) e Chave Estrangeira
        [ForeignKey("Anuncio")]
        public int id_anuncio { get; set; }

        public DateTime data { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal preco { get; set; }
        public string estado_pagamento { get; set; } // pendente, pago, cancelado

        // Propriedades de navegação
        public virtual Comprador Comprador { get; set; }
        public virtual Anuncio Anuncio { get; set; }
    }
}