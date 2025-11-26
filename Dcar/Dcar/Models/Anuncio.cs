using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Anuncio
{
    public int IdAnuncio { get; set; }

    public int IdVendedor { get; set; }

    public int IdCarro { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descricao { get; set; }

    public DateOnly DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public string? Localizacao { get; set; }

    public decimal Preco { get; set; }

    public string Estado { get; set; } = null!;

    public string? Fotos { get; set; }

    public virtual ICollection<Agendum> Agenda { get; set; } = new List<Agendum>();

    public virtual Compra? Compra { get; set; }

    public virtual ICollection<Denuncium> Denuncia { get; set; } = new List<Denuncium>();

    public virtual Carro IdCarroNavigation { get; set; } = null!;

    public virtual Vendedor IdVendedorNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
