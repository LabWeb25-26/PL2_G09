using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Carro
{
    public int IdCarro { get; set; }

    public string? Vin { get; set; }

    public string Matricula { get; set; } = null!;

    public int Ano { get; set; }

    public int Quilometragem { get; set; }

    public string? Caixa { get; set; }

    public int IdModelo { get; set; }

    public int IdCombustivel { get; set; }

    public virtual Anuncio? Anuncio { get; set; }

    public virtual Combustivel IdCombustivelNavigation { get; set; } = null!;

    public virtual Modelo IdModeloNavigation { get; set; } = null!;
}
