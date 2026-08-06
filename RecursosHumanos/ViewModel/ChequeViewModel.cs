using RecursosHumanos.Models;

namespace RecursosHumanos.ViewModel {
    public class ChequeViewModel {
        public int Ejercicio { get; set; }
        public int Quincena { get; set; }
        public int UltimoDigito { get; set; }
        public CustomTable Cheques { get; set; }
    }
}
