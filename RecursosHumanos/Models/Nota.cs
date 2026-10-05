using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecursosHumanos.Models {
    public class Nota {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; } = string.Empty;
        [Required]
        public string Contenido { get; set; } = string.Empty;
        // Relación con TipoNota
        [ForeignKey("TipoNota")]
        public int TipoNotaId { get; set; }
        public TipoNota? TipoNota { get; set; }
        // Si es para todos los usuarios
        public bool EsParaTodos { get; set; }
        // Si no es para todos, se asigna a un usuario específico (Identity)
        public string? UsuarioId { get; set; }
        // Fecha de la nota (para calendario)
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public DateTime Creado { get; set; } = DateTime.UtcNow;
        public DateTime? Editado { get; set; }
        public DateTime? Eliminado { get; set; }
        // Usuario que realizó la acción (Identity User Id)
        public string? CreadoPor { get; set; }
        public string? EditadoPor { get; set; }
        public string? EliminadoPor { get; set; }
    }
}
