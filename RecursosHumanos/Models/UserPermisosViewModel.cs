using System.Collections.Generic;

namespace RecursosHumanos.Models
{
    public class UserPermisosViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PermisoSelection> Permisos { get; set; } = new List<PermisoSelection>();
    }

    public class PermisoSelection
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }
}
