using System.Collections.Generic;

namespace DCarMarketplace.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Totais Gerais
        public int TotalUtilizadores { get; set; }
        public int TotalVendedores { get; set; }
        public int TotalCompradores { get; set; }

        // Anúncios
        public int AnunciosAtivos { get; set; }
        public int AnunciosVendidos { get; set; }

        // Financeiro
        public int TotalVendasRealizadas { get; set; }
        public decimal ValorTotalTransacionado { get; set; }

        // Tops (Listas para gráficos ou tabelas)
        public Dictionary<string, int> TopMarcas { get; set; } // Ex: "BMW" -> 10
    }
}