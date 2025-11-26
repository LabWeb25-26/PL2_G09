using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Denuncium
{
    public int IdDenuncia { get; set; }

    public string IdDenuncianteUser { get; set; } = null!;

    public int? IdAlvoAnuncio { get; set; }

    public DateTime DataAbertura { get; set; }

    public string Motivo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual Anuncio? IdAlvoAnuncioNavigation { get; set; }
}
