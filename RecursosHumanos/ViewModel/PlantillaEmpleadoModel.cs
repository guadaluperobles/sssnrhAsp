using RecursosHumanos.Model;
using System.Data;

namespace RecursosHumanos.ViewModel {
    public class PlantillaEmpleadoModel {
        public string NumeroEmpleado { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string NSS { get; set; }
        public string ISSTE { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Nombre { get; set; }
        public string NombreCompleto { get; set; }
        public string Puesto { get; set; }
        public string NumeroPuesto { get; set; }
        public string DescripcionPuesto { get; set; }
        public string CentroTrabajo { get; set; }
        public string DescripcionCentroTrabajo { get; set; }
        public string CentroDistribucion { get; set; }
        public string DescripcionCentroDistribucion { get; set; }
        /**
         * PRIMERA POSICION    
         * 1 = Registro con proceso posterior    
         * 2 = Registro sin proceso posterior 
         * 
         * SEGUNDA POSICION    
         * 0 = activo    
         * 1 = licencia (genera pago)    
         * 2 = licencia (no genera pago)    
         * 3 = baja temporal (no genera pago)   
         * 4 = baja definitiva    
         * 5 = baja con marca de contraloria
         */
        public string Estatus { get; set; }
        public string ur { get; set; }
        public string feiinst { get; set; }
        public string feiram { get; set; }
        public string fecnac { get; set; }
        public string Movimiento { get; set; }
        public string fecham { get; set; }
        public string cvepag { get; set; }
        public string BaseDatos { get; set; }
        public string sexo { get; set; }
        public string hijos { get; set; }
        public string SueldoBruto { get; set; }
        public string SueldoNeto { get; set; }
        public string horario { get; set; }
        public string dlabi { get; set; }
        public string dlabr { get; set; }
        public string cluest { get; set; }
        public string cluesd { get; set; }
        public string uadmva { get; set; }
        public string Antiguedad { get; set; }
        /**
         * tpuesto
         * PRIMERA POSICION RAMA    
         * 1 = presupuestal   
         * 2 = eventual    
         * 3 = lista de raya    
         * 4 = medica 
         * SEGUNDA POSICION TIPO DE PUESTO     
         * 1 = base    
         * 2 = confianza    
         * 3 = honorarios    
         * 4 = medico residente    
         * 5 = medico interno de pregrado    
         * 6 = pasante en servicio social 
         * TERCERA POSICION TIPO DE CONT    
         * 1 = propiedad    
         * 2 = interina limitada    
         * 3 = provisional
         */
        public string tpuesto { get; set; }
        public string InstrumentoPago { get; set; }
        public string CuentaBanco { get; set; }
        public string PuestoSHCP { get; set; }
        public string NivelAcademico { get; set; }
        public string turno { get; set; }
        public string Programa { get; set; }
        public string MeSATCPost { get; set; }
        public string SATNombre { get; set; }
        public string satdice { get; set; }
    }
}
