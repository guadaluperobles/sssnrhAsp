using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursosHumanos.Models {
    public class Cheque {
        private int? _Id; 
        private int? _ClkDet; 
        private string _NombreEmpleado;
        private string _NombreBeneficiario;
        private string _RfcEmpleado;
        private int? _Folio;
        private string _ClavePresupuestal;
        private int? _NumeroCheque;
        private string _ClaveUbicacion;
        private string _Descripcion;
        private string _InicioPeriodo;
        private string _FinPeriodo;
        private string _FechaPago;
        private double? _Neto;
        private double? _Deducciones;
        private double? _VPA;
        private string _VPATexto;
        private short? _impreso;
        private int? _Ejercicio;
        private int? _Quincena;
        private DateTime _Creado;
        private DateTime? _Editado;
        private DateTime? _Eliminado;
        private DateTime? _Impreso;

        public int? Id { get { return _Id; } set { _Id = value; } }
        public int? ClkDet { get { return _ClkDet; } set { _ClkDet = value; } }
        public string NombreEmpleado { get { return _NombreEmpleado; } set { _NombreEmpleado = value; } }
        public string NombreBeneficiario { get { return _NombreBeneficiario; } set { _NombreBeneficiario = value; } }
        public string RfcEmpleado { get { return _RfcEmpleado; } set { _RfcEmpleado = value; } }
        public int? Folio { get { return _Folio; } set { _Folio = value; } }
        public string ClavePresupuestal { get { return _ClavePresupuestal; } set { _ClavePresupuestal = value; } }
        public int? NumeroCheque { get { return _NumeroCheque; } set { _NumeroCheque = value; } }
        public string ClaveUbicacion { get { return _ClaveUbicacion; } set { _ClaveUbicacion = value; } }
        public string Descripcion { get { return _Descripcion; } set { _Descripcion = value; } }
        public string InicioPeriodo { get { return _InicioPeriodo; } set { _InicioPeriodo = value; } }
        public string FinPeriodo { get { return _FinPeriodo; } set { _FinPeriodo = value; } }
        public string FechaPago { get { return _FechaPago; } set { _FechaPago = value; } }
        public double? Neto { get { return _Neto; } set { _Neto = value; } }
        public double? Deducciones { get { return _Deducciones; } set { _Deducciones = value; } }
        public double? VPA { get { return _VPA; } set { _VPA = value; } }
        public string VPATexto { get { return _VPATexto; } set { _VPATexto = value; } }
        public short? impreso { get { return _impreso; } set { _impreso = value; } }
        public int? Ejercicio { get { return _Ejercicio; } set { _Ejercicio = value; } }
        public int? Quincena { get { return _Quincena; } set { _Quincena = value; } }
        public DateTime Creado { get { return _Creado; } set { _Creado = value; } }
        public DateTime? Editado { get { return _Editado; } set { _Editado = value; } }
        public DateTime? Eliminado { get { return _Eliminado; } set { _Eliminado = value; } }
        public DateTime? Impreso { get { return _Impreso; } set { _Impreso = value; } }
    }
}
