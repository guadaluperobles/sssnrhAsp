using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Models;

namespace RecursosHumanos.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RecursosHumanos.Models.Consultas> Consulta { get; set; } = default!;
        public DbSet<RecursosHumanos.Models.Cheque> Cheque { get; set; } = default!;
        public DbSet<RecursosHumanos.Models.PermisoVistaModel> PermisoVistaModel { get; set; } = default!;
        public DbSet<RecursosHumanos.Models.UsuarioPermiso> UsuarioPermiso { get; set; } = default!;
        public DbSet<RecursosHumanos.Models.TipoNota> TipoNota { get; set; } = default!;
        public DbSet<RecursosHumanos.Models.Nota> Nota { get; set; } = default!;

        // Nuevos módulos
        public DbSet<EnvioPlantilla> EnvioPlantilla { get; set; } = default!;
        public DbSet<EmpleadoEstatal> EmpleadoEstatal { get; set; } = default!;
    }
}
