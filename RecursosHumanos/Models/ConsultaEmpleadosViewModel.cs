using System.Data;

namespace RecursosHumanos.Models {
    public class ConsultaEmpleadosViewModel {
        public DataTable BasesDatos { get; set; }
        public CustomTable Empleados { get; set; }
        public string Localizar { get; set; }
    }
}
