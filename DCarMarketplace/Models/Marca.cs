using System.ComponentModel.DataAnnotations;

namespace DCarMarketplace.Models
{
    public class Marca
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        // Uma marca tem muitos modelos
        public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
        // Uma marca pode ser favorita de muitos compradores
        public virtual ICollection<MarcaFavorita> Favoritos { get; set; } = new List<MarcaFavorita>();
    }
}
