using System;
using System.Collections.Generic;

namespace Dcar.Models;

public partial class Administrador
{
    public int IdAdmin { get; set; }

    public string UserId { get; set; } = null!;

    public string? Nome { get; set; }
}
