using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Modelo
{
    public int IdModelo { get; set; }

    public int IdMarca { get; set; }

    public string NomeModelo { get; set; } = null!;

    public virtual ICollection<Carro> Carros { get; set; } = new List<Carro>();

    public virtual Marca IdMarcaNavigation { get; set; } = null!;
}
