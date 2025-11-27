using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Vendedor
    {
        [Key, ForeignKey("Utilizador")]
        public string Id { get; set; }

        public string? Morada { get; set; }
        public string? Contactos { get; set; }
        public string? Tipo { get; set; } // 'Particular' ou 'Empresa'
        public string? NIF { get; set; }
        public string EstadoAprovacao { get; set; } = "pendente"; // 'pendente', 'aprovado'

        public virtual Utilizador Utilizador { get; set; }

        // Um vendedor tem muitos anúncios
        public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();
    }
}
