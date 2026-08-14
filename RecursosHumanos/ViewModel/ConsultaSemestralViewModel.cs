using RecursosHumanos.Models;
using System.Data;

namespace RecursosHumanos.ViewModel {
    public class ConsultaSemestralViewModel {
        public int Ejercicio { get; set; }
        public int Semestre { get; set; }
        public int Inicial { get; set; }
        public int Final { get; set; }
        public String Controlador { get; set; }
        public DataTable Models { get; set; }
    }
}
