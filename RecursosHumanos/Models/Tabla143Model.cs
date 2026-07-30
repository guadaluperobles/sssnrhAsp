using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorRecursosHumanos.Model {
    //Sistemas de Compenzacion
    class Tabla143Model {
        private string _id;
        private string _denominacion;
        private string _montoBruto;
        private string _montoNeto;
        private string _tipoMoneda;
        private string _periodicidad;
        public string Id { get { return _id; } set { _id = value; } }
        public string Denominacion { get { return _denominacion; } set { _denominacion = value; } }
        public string MontoBruto { get { return _montoBruto; } set { _montoBruto = value; } }
        public string MontoNeto { get { return _montoNeto; } set { _montoNeto = value; } }
        public string TipoMoneda { get { return _tipoMoneda; } set { _tipoMoneda = value; } }
        public string Periodicidad { get { return _periodicidad; } set { _periodicidad = value; } }
    }
}
