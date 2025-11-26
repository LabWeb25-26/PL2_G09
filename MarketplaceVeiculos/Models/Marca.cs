using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <--- Adicione isto

namespace MarketplaceVeiculos.Models
{
    [Table("Marca")] // <--- Adicione isto (corrige o erro "Invalid object name")
    public class Marca
    {
        [Key]
        public int id_marca { get; set; }
        public string nome_marca { get; set; }
    }
}