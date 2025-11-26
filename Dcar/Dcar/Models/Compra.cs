using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Compra
{
    public int IdCompra { get; set; }

    public int IdComprador { get; set; }

    public int IdAnuncio { get; set; }

    public DateTime Data { get; set; }

    public decimal Preco { get; set; }

    public string EstadoPagamento { get; set; } = null!;

    public virtual Anuncio IdAnuncioNavigation { get; set; } = null!;

    public virtual Comprador IdCompradorNavigation { get; set; } = null!;
}
