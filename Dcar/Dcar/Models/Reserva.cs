using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int IdAnuncio { get; set; }

    public int IdComprador { get; set; }

    public DateOnly Data { get; set; }

    public string Estado { get; set; } = null!;

    public DateOnly? PrazoExpiracao { get; set; }

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}
