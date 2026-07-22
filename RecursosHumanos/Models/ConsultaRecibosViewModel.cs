using System.Data;

namespace RecursosHumanos.Models {
    public class ConsultaRecibosViewModel {
        public DataTable BasesDatos { get; set; }
        public DataTable Recibos { get; set; }
        public string NumeroEmpleado { get; set; }
        public string EjercicioInicio { get; set; }
        public string EjercicioFin { get; set; }
        public string QuincenaInicio { get; set; }
        public string QuincenaFin { get; set; }
    }
}
