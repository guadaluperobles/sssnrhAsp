using RecursosHumanos.Models;
using System.Data;

namespace RecursosHumanos.ViewModel {
    public class EmpleadosConsulta {
        public DataTable BasesDatos { get; set; }
        public CustomTable Empleados { get; set; }
        public string Localizar { get; set; }
    }
}
