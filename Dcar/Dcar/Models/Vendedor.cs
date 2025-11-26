using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Vendedor
{
    public int IdVendedor { get; set; }

    public string UserId { get; set; } = null!;

    public string? Nome { get; set; }

    public string? Morada { get; set; }

    public string? Contactos { get; set; }

    public string? Tipo { get; set; }

    public string? Nif { get; set; }

    public string EstadoAprovacao { get; set; } = null!;

    public virtual ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();
}
