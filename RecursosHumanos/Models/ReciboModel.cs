using GeneradorRecursosHumanos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorRecursosHumanos.Model
{
    public class ReciboModel {
        private string? _noComprobante;
        private string? _quincena;
        private string? _fechaDePago;
        private string? _periodoDePago;
        private string? _noSeguridadSocial;
        private string? _folioFiscal;
        private string? _regimenFiscal;
        private string? _fechaEmision;
        private string? _nombre;
        private string? _noEmpleado;
        private string? _rfc;
        private string? _CURP;
        private string? _centroDeTrabajo;
        private string? _totalPercepciones;
        private string? _totalDeducciones;
        private string? _totalPagar;
        private string? _puesto;
        private string? _total;
        private string? _importeLetras;
        private string? _cadenaOriginalSAT;
        private string? _selloDigitalSAT;
        private string? _certificadoSAT;
        private string? _selloDigitalCFDI;
        private string? _version;
        private string? _rfcProveedor;
        private string? _fechaHoraCertificacion;
        private string? _qrVerificador;
        private string? _antiguedad;
        private string? _XML;
        private string? _NombreArchivo;
        private string? _BaseDatos;
        private byte[]   _CodigoQR;
        private List<DeduccionModel> _Deducciones;
        private List<Percepcion> _Percepciones;

        public string noComprobante { get { return _noComprobante; } set { _noComprobante = value; } }
        public string quincena { get { return _quincena; } set { _quincena = value; } }
        public string fechaDePago { get { return _fechaDePago; } set { _fechaDePago = value; } }
        public string periodoDePago { get { return _periodoDePago; } set { _periodoDePago = value; } }
        public string noSeguridadSocial { get { return _noSeguridadSocial; } set { _noSeguridadSocial = value; } }
        public string folioFiscal { get { return _folioFiscal; } set { _folioFiscal = value; } }
        public string regimenFiscal { get { return _regimenFiscal; } set { _regimenFiscal = value; } }
        public string fechaEmision { get { return _fechaEmision; } set { _fechaEmision = value; } }
        public string nombre { get { return _nombre; } set { _nombre = value; } }
        public string noEmpleado { get { return _noEmpleado; } set { _noEmpleado = value; } }
        public string rfc { get { return _rfc; } set { _rfc = value; } }
        public string CURP { get { return _CURP; } set { _CURP = value; } }
        public string puesto { get { return _puesto; } set { _puesto = value; } }
        public string centroDeTrabajo { get { return _centroDeTrabajo; } set { _centroDeTrabajo = value; } }
        public string totalPercepciones { get { return _totalPercepciones; } set { _totalPercepciones = value; } }
        public string totalDeducciones { get { return _totalDeducciones; } set { _totalDeducciones = value; } }
        public string totalPagar { get { return _totalPagar; } set { _totalPagar = value; } }
        public string total { get { return _total; } set { _total = value; } }
        public string importeLetras { get { return _importeLetras; } set { _importeLetras = value; } }
        public string cadenaOriginalSAT { get { return _cadenaOriginalSAT; } set { _cadenaOriginalSAT = value; } }
        public string selloDigitalSAT { get { return _selloDigitalSAT; } set { _selloDigitalSAT = value; } }
        public string certificadoSAT { get { return _certificadoSAT; } set { _certificadoSAT = value; } }
        public string selloDigitalCFDI { get { return _selloDigitalCFDI; } set { _selloDigitalCFDI = value; } }
        public string version { get { return _version; } set { _version = value; } }
        public string rfcProveedor { get { return _rfcProveedor; } set { _rfcProveedor = value; } }
        public string fechaHoraCertificacion { get { return _fechaHoraCertificacion; } set { _fechaHoraCertificacion = value; } }
        public string qrVerificador { get { return _qrVerificador; } set { _qrVerificador = value; } }
        public string antiguedad { get { return _antiguedad; } set { _antiguedad = value; } }
        public string XML { get { return _XML; } set { _XML = value; } }
        public string NombreArchivo { get { return _NombreArchivo; } set { _NombreArchivo = value; } }
        public string BaseDatos { get { return _BaseDatos; } set { _BaseDatos = value; } }
        public byte[] CodigoQR { get { return _CodigoQR; } set { _CodigoQR = value; } }
        public List<DeduccionModel> Deducciones { get { return _Deducciones; } set { _Deducciones = value; } }
        public List<Percepcion> Percepciones { get { return _Percepciones; } set { _Percepciones = value; } }
    }
}
