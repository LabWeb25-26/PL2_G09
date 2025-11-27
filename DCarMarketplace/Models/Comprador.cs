using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCarMarketplace.Models
{
    public class Comprador
    {
        [Key, ForeignKey("Utilizador")]
        public string Id { get; set; }

        public string? Morada { get; set; }
        public string? Contactos { get; set; }

        public virtual Utilizador Utilizador { get; set; }

        // As várias interações do comprador
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public virtual ICollection<Agenda> VisitasAgendadas { get; set; } = new List<Agenda>();
        public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
        public virtual ICollection<FiltroFavorito> FiltrosFavoritos { get; set; } = new List<FiltroFavorito>();
        public virtual ICollection<MarcaFavorita> MarcasFavoritas { get; set; } = new List<MarcaFavorita>();
    }
}