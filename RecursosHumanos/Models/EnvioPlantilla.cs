using System;
using System.ComponentModel.DataAnnotations;

namespace RecursosHumanos.Models
{
    public class EnvioPlantilla
    {
        [Key]
        public string Id { get; set; } = null!;
        public string Correo { get; set; } = string.Empty;
        public string Plantilla { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public DateTime creado { get; set; }
        public DateTime modificado { get; set; }
        public DateTime eliminado { get; set; }
    }
}
