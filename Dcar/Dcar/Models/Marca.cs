using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string NomeMarca { get; set; } = null!;

    public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
}
