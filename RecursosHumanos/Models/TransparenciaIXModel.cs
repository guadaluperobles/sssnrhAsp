using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneradorRecursosHumanos.Model {
    class TransparenciaIXModel {
        private string _Ejercicio;
        private string _FechaInicio;
        private string _FechaFin;
        private string _DenominacionArea;
        private string _DenominacionPuesto;
        private string _NivelPuesto;
        private string _AreaAdcripcion;
        private string _Sexo;
        private string _HipervinculoConvocatoria;
        private string _AreaResponsable;
        private string _FechaActualizacion;
        private string _Nota;

        //Ejercicio 
        public string Ejercicio { get { return _Ejercicio; } set { _Ejercicio = value; } }
        //Fecha de inicio del periodo que se informa
        public string FechaInicio { get { return _FechaInicio; } set { _FechaInicio = value; } }
        //Fecha de término del periodo que se informa 
        public string FechaFin { get { return _FechaFin; } set { _FechaFin = value; } }
        //Denominación del área   
        public string DenominacionArea { get { return _DenominacionArea; } set { _DenominacionArea = value; } }
        //Denominación del puesto( Redactados con perspectiva de género)  
        public string DenominacionPuesto { get { return _DenominacionPuesto; } set { _DenominacionPuesto = value; } }
        //Clave o nivel de puesto Tipo de plaza( catálogo)    
        public string NivelPuesto { get { return _NivelPuesto; } set { _NivelPuesto = value; } }
        //Área de adscripción Por cada puesto y/o cargo de la estructura especificar el estado( catálogo) 
        public string AreaAdcripcion { get { return _AreaAdcripcion; } set { _AreaAdcripcion = value; } }
        //Sexo( catálogo) 
        public string Sexo { get { return _Sexo; } set { _Sexo = value; } }
        //Por cada puesto y/o cargo de la estructura vacante se incluirá un hipervínculo a las convocatorias a concursos para ocupar cargos públicos( Redactadas con perspectiva de género)   
        public string HipervinculoConvocatoria { get { return _HipervinculoConvocatoria; } set { _HipervinculoConvocatoria = value; } }
        //Área( s) responsable( s) que genera( n), posee( n), publica( n) y actualiza( n) la información 
        public string AreaResponsable { get { return _AreaResponsable; } set { _AreaResponsable = value; } }
        //Fecha de actualización 
        public string FechaActualizacion { get { return _FechaActualizacion; } set { _FechaActualizacion = value; } }
        //Nota
        public string Nota { get { return _Nota; } set { _Nota = value; } }
    }
}
