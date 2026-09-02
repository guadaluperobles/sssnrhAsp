using RecursosHumanos.Models;
using System.Data;

namespace RecursosHumanos.ViewModel {
    public class NominaViewModel {
        public string BaseDatos { get; set; }
        public string Producto { get; set; }
        public int Quincena { get; set; }
        public DataTable Procesados { get; set; }
        public DataTable ProcesadosFaltantes { get; set; }
        public DataTable EmpleadosFaltantes { get; set; }
        public DataTable BasesDatos { get; set; }
        public DataTable Models { get; set; }
        public DataRow Model { get; set; }
        public DataTable ProcesadosJson { get; set; } = new DataTable();
    }
}
