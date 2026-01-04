using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Administrador{
    [Key, ForeignKey("Utilizador")]
    public string Id { get; set; }
                                                                                                                                                             
    public virtual Utilizador Utilizador { get; set; }

    // Auditoria: Ações que este administrador executou
    [InverseProperty("Admin")]
    public virtual ICollection<HistoricoAcaoAdmin> AcoesRealizadas { get; set; } = new List<HistoricoAcaoAdmin>();
    }
}
