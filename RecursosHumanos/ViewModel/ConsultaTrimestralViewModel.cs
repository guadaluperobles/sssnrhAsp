using RecursosHumanos.Models;
using System.Data;

namespace RecursosHumanos.ViewModel {
    public class ConsultaTrimestralViewModel {
        public int Ejercicio { get; set; }
        public int Trimestre { get; set; }
        public String Controlador { get; set; }
        public DataTable Models { get; set; }
    }
}
