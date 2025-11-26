using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Comprador
{
    public int IdComprador { get; set; }

    public string UserId { get; set; } = null!;

    public string? Nome { get; set; }

    public string? Morada { get; set; }

    public string? Contactos { get; set; }

    public virtual ICollection<Agendum> Agenda { get; set; } = new List<Agendum>();

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
