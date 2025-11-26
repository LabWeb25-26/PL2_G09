using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Agendum
{
    public int IdAgenda { get; set; }

    public int IdComprador { get; set; }

    public int IdAnuncio { get; set; }

    public DateOnly DataAgenda { get; set; }

    public DateTime DataVisita { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}
