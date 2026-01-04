using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Modelo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MarcaId { get; set; }
                                                                                                                                                                                                                                                                                                
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [ForeignKey("MarcaId")]
        public virtual Marca Marca { get; set; }

        public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();
    }
}
