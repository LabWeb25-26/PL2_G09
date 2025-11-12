using System.ComponentModel.DataAnnotations.Schema;

namespace MarketplaceVeiculos.Models
{
    [Table("Administrador")]
    public class Administrador : Utilizador
    {
        // Esta classe pode ter propriedades específicas do admin, 
        // mas por agora apenas herda do Utilizador.
    }
}