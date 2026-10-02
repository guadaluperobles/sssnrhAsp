using System;

namespace RecursosHumanos.Models
{
    public class UsuarioPermiso
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int? PermisoId { get; set; }
        public DateTime Creado { get; set; } = DateTime.UtcNow;
        public DateTime? Editado { get; set; }
        public DateTime? Eliminado { get; set; }

        public PermisoVistaModel? Permiso { get; set; }
    }
}
