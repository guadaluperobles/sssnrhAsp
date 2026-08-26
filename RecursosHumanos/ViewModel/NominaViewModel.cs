using RecursosHumanos.Models;
using System.Data;

namespace RecursosHumanos.ViewModel {
    public class NominaViewModel {
        public string BaseDatos { get; set; }
        public int Quincena { get; set; }
        public DataTable Procesados { get; set; }
        public DataTable ProcesadosFaltantes { get; set; }
        public DataTable EmpleadosFaltantes { get; set; }
    }
}
