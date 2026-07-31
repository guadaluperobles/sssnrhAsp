using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursosHumanos.Models {
    public class DeduccionModel {
        private string _Clave;
        private string _Concepto;
        private string _TextImporte;
        private Decimal _Importe;
        public string Clave { get { return _Clave; } set { _Clave = value; } }
        public string Concepto { get { return _Concepto; } set { _Concepto = value; } }
        public string TextImporte { get { return _TextImporte; } set { _TextImporte = value; } }
        public Decimal Importe { get { return _Importe; } set { _Importe = value; } }
    }
}
