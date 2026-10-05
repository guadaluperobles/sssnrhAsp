using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;
using System.Diagnostics;

namespace RecursosHumanos.Controllers {
    public class HomeController : Controller {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager) {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index() {

            var notas = await _context.Nota.Include(n => n.TipoNota).Where(n => n.Eliminado == null).ToListAsync();
            var eventos = notas.Select(n => new EventoModel {
                id = n.Id,
                title = n.Titulo,
                description = n.Contenido,
                start = n.Fecha,
                backgroundColor = $"rgb({n.TipoNota.ColorR},{n.TipoNota.ColorG},{n.TipoNota.ColorB})",
                borderColor= $"rgb({n.TipoNota.ColorR},{n.TipoNota.ColorG},{n.TipoNota.ColorB})",
            }).ToList();

            return View(eventos);
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
