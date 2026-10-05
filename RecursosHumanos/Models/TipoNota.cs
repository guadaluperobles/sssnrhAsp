using System;
using System.ComponentModel.DataAnnotations;

namespace RecursosHumanos.Models
{
    public class TipoNota
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        // Color en RGB (0-255)
        public int ColorR { get; set; }
        public int ColorG { get; set; }
        public int ColorB { get; set; }

        public DateTime Creado { get; set; } = DateTime.UtcNow;
        public DateTime? Editado { get; set; }
        public DateTime? Eliminado { get; set; }
        // Usuario que realizó la acción (Identity User Id)
        public string? CreadoPor { get; set; }
        public string? EditadoPor { get; set; }
        public string? EliminadoPor { get; set; }
    }
}
