using System.Collections.Generic;
using DCarMarketplace.Models; // Garante que tens este using

namespace DCarMarketplace.Models.ViewModels
{
    public class HistoricoCompradorViewModel
    {
        public List<Reserva> MinhasReservas { get; set; }
        public List<Agenda> MinhasVisitas { get; set; }

        // NOVO: Lista de Compras
        public List<Compra> MinhasCompras { get; set; }
    }
}