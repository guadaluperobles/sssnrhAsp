using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RecursosHumanos.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
    public DbSet<RecursosHumanos.Models.Cheque> Cheque { get; set; } = default!;
    }
}
