using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Combustivel
{
    public int IdCombustivel { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();
}
