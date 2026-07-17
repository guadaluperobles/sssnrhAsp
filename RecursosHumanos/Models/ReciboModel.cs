using GeneradorRecursosHumanos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorRecursosHumanos.Model
{
    public class ReciboModel
    {
        public string noComprobante { get; set; }
        public string quincena { get; set; }
        public string fechaDePago { get; set; }
        public string periodoDePago { get; set; }
        public string noSeguridadSocial { get; set; }
        public string folioFiscal { get; set; }
        public string regimenFiscal { get; set; }
        public string fechaEmision { get; set; }
        public string nombre { get; set; }
        public string noEmpleado { get; set; }
        public string rfc { get; set; }
        public string CURP { get; set; }
        public string puesto { get; set; }
        public string centroDeTrabajo { get; set; }
        public string totalPercepciones { get; set; }
        public string totalDeducciones { get; set; }
        public string totalPagar { get; set; }
        public string total { get; set; }
        public string importeLetras { get; set; }
        public string cadenaOriginalSAT { get; set; }
        public string selloDigitalSAT { get; set; }
        public string certificadoSAT { get; set; }
        public string selloDigitalCFDI { get; set; }
        public string version { get; set; }
        public string rfcProveedor { get; set; }
        public string fechaHoraCertificacion { get; set; }
        public string qrVerificador { get; set; }
        public string antiguedad { get; set; }
        public string XML { get; set; }
        public string NombreArchivo { get; set; }
        public string BaseDatos { get; set; }
        public byte[] CodigoQR { get; set; }
        public List<DeduccionModel> deducciones { get; set; }
        public List<Percepcion> percepciones { get; set; }
    }
}
