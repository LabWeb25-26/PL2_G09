using System.ComponentModel.DataAnnotations;

namespace DCarMarketplace.Models
{
    public class Combustivel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } // Gasolina, Diesel, Elétrico...

        public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();
    }
}
