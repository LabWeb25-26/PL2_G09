using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Anuncio
    {
        [Key]
        public int Id { get; set; }

        // Quem vende e o quê
        [Required]
        public string VendedorId { get; set; }
        [Required]
        public int CarroId { get; set; }
                                                                                                                                                                                           
        [Required]
        public string Titulo { get; set; }
        public string Descricao { get; set; }

        public DateTime DataInicio { get; set; } = DateTime.Now;
        public DateTime? DataFim { get; set; }

        public string Localizacao { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        // 'ativo', 'reservado', 'vendido', 'pausado'
        [Required]
        public string Estado { get; set; }

        public List<Foto> Fotos { get; set; } = new List<Foto>();

        // Navegação
        [ForeignKey("VendedorId")]
        public virtual Vendedor Vendedor { get; set; }

        [ForeignKey("CarroId")]
        public virtual Carro Carro { get; set; }

        // Listas de interações com este anúncio
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public virtual ICollection<Agenda> Visitas { get; set; } = new List<Agenda>();
       

        // Relação 1-para-0..1 com Compra (só pode ser comprado uma vez)
        public virtual Compra? Compra { get; set; }
    }
}
