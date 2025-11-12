using System.ComponentModel.DataAnnotations;

namespace MarketplaceVeiculos.Models
{
    public class Utilizador
    {
        [Key]
        public int id_utilizador { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string email { get; set; }

        [Required(ErrorMessage = "O username é obrigatório")]
        public string username { get; set; }

        [Required(ErrorMessage = "A password é obrigatória")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        public string? estado_conta { get; set; } = "ativo";
    }
}