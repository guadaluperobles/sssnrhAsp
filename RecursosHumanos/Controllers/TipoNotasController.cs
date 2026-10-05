using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;

namespace RecursosHumanos.Controllers
{
    public class TipoNotasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TipoNotasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TipoNotas
        public async Task<IActionResult> Index()
        {
            var items = await _context.TipoNota.Where(t => t.Eliminado == null).ToListAsync();
            return View(items);
        }

        // GET: TipoNotas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tipo = await _context.TipoNota.FirstOrDefaultAsync(m => m.Id == id && m.Eliminado == null);
            if (tipo == null) return NotFound();

            return View(tipo);
        }

        // GET: TipoNotas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoNotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,ColorR,ColorG,ColorB")] TipoNota tipoNota)
        {
            if (ModelState.IsValid)
            {
                tipoNota.Creado = DateTime.UtcNow;
                tipoNota.CreadoPor = _userManager.GetUserId(User);
                _context.Add(tipoNota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoNota);
        }

        // GET: TipoNotas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipo = await _context.TipoNota.FindAsync(id);
            if (tipo == null || tipo.Eliminado != null) return NotFound();
            return View(tipo);
        }

        // POST: TipoNotas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,ColorR,ColorG,ColorB")] TipoNota tipoNota)
        {
            if (id != tipoNota.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var entity = await _context.TipoNota.FindAsync(id);
                if (entity == null) return NotFound();

                entity.Nombre = tipoNota.Nombre;
                entity.ColorR = tipoNota.ColorR;
                entity.ColorG = tipoNota.ColorG;
                entity.ColorB = tipoNota.ColorB;
                entity.Editado = DateTime.UtcNow;
                entity.EditadoPor = _userManager.GetUserId(User);

                _context.Update(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoNota);
        }

        // GET: TipoNotas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tipo = await _context.TipoNota.FirstOrDefaultAsync(m => m.Id == id && m.Eliminado == null);
            if (tipo == null) return NotFound();

            return View(tipo);
        }

        // POST: TipoNotas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipo = await _context.TipoNota.FindAsync(id);
            if (tipo == null) return NotFound();

            tipo.Eliminado = DateTime.UtcNow;
            tipo.EliminadoPor = _userManager.GetUserId(User);
            _context.Update(tipo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
