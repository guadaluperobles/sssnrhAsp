using Microsoft.AspNetCore.Mvc;

namespace RecursosHumanos.Models {
    public class CustomTable {
        public IEnumerable<object>? Datos { get; set; }
        public List<CustomTableColumn>? Columnas { get; set; }
        public int PaginaActual { get; set; }
        public int RegistrosPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas =>
            (int)Math.Ceiling(
                (double)TotalRegistros / RegistrosPorPagina
            );
    }
}