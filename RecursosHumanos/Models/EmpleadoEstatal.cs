using System;
using System.ComponentModel.DataAnnotations;

namespace RecursosHumanos.Models
{
    public class EmpleadoEstatal
    {
        [Key]
        public string Id { get; set; } = null!;

        public string nemp { get; set; } = string.Empty;
        public string pension { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string paterno { get; set; } = string.Empty;
        public string materno { get; set; } = string.Empty;
        public string curp { get; set; } = string.Empty;
        public string rfc { get; set; } = string.Empty;
        public string sexo { get; set; } = string.Empty;
        public string escol { get; set; } = string.Empty;
        public string fecnac { get; set; } = string.Empty;
        public string email1 { get; set; } = string.Empty;
        public string email2 { get; set; } = string.Empty;
        public string tipoemp { get; set; } = string.Empty;
        public string sind { get; set; } = string.Empty;
        public string estpza { get; set; } = string.Empty;
        public string pzavac { get; set; } = string.Empty;
        public string recpza { get; set; } = string.Empty;
        public string numplaza { get; set; } = string.Empty;
        public string cvepago { get; set; } = string.Empty;
        public string fecing { get; set; } = string.Empty;
        public string nivel { get; set; } = string.Empty;
        public string puesto { get; set; } = string.Empty;
        public string ptoficial { get; set; } = string.Empty;
        public string direccion { get; set; } = string.Empty;
        public string mpio { get; set; } = string.Empty;
        public string sb { get; set; } = string.Empty;
        public string quin { get; set; } = string.Empty;
        public string comp { get; set; } = string.Empty;
        public string otp { get; set; } = string.Empty;
        public string sbm { get; set; } = string.Empty;
        public string snm { get; set; } = string.Empty;
        public string prctrab { get; set; } = string.Empty;
        public string ctdsc { get; set; } = string.Empty;
        public string prctrabdis { get; set; } = string.Empty;
        public string ctdsc1 { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;

        public DateTime creado { get; set; }
        public DateTime modificado { get; set; }
        public DateTime eliminado { get; set; }
    }
}
