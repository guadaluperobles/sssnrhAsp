using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RecursosHumanos.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
    public DbSet<RecursosHumanos.Models.Consultas> Consulta { get; set; } = default!;
    public DbSet<RecursosHumanos.Models.Cheque> Cheque { get; set; } = default!;
    public DbSet<RecursosHumanos.Models.PermisoVistaModel> PermisoVistaModel { get; set; } = default!;
    public DbSet<RecursosHumanos.Models.UsuarioPermiso> UsuarioPermiso { get; set; } = default!;
    public DbSet<RecursosHumanos.Models.TipoNota> TipoNota { get; set; } = default!;
    public DbSet<RecursosHumanos.Models.Nota> Nota { get; set; } = default!;
    }
}
