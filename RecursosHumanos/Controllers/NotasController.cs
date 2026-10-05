using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;

namespace RecursosHumanos.Controllers
{
    public class NotasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Notas
        public async Task<IActionResult> Index()
        {
            var notas = await _context.Nota.Include(n => n.TipoNota).Where(n => n.Eliminado == null).ToListAsync();
            return View(notas);
        }

        // GET: Notas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var nota = await _context.Nota.Include(n => n.TipoNota).FirstOrDefaultAsync(m => m.Id == id && m.Eliminado == null);
            if (nota == null) return NotFound();

            return View(nota);
        }

        // GET: Notas/Create
        public async Task<IActionResult> Create()
        {
            ViewData["TipoNotaId"] = new SelectList(await _context.TipoNota.Where(t => t.Eliminado == null).ToListAsync(), "Id", "Nombre");
            ViewData["Usuarios"] = await _userManager.Users.ToListAsync();
            var model = new Nota { Fecha = DateTime.Now };
            return View(model);
        }

        // POST: Notas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Contenido,TipoNotaId,EsParaTodos,UsuarioId,Fecha")] Nota nota)
        {
            if (ModelState.IsValid)
            {
                nota.Creado = DateTime.UtcNow;
                nota.CreadoPor = _userManager.GetUserId(User);
                _context.Add(nota);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoNotaId"] = new SelectList(await _context.TipoNota.Where(t => t.Eliminado == null).ToListAsync(), "Id", "Nombre", nota.TipoNotaId);
            ViewData["Usuarios"] = await _userManager.Users.ToListAsync();
            return View(nota);
        }

        // GET: Notas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var nota = await _context.Nota.FindAsync(id);
            if (nota == null || nota.Eliminado != null) return NotFound();

            ViewData["TipoNotaId"] = new SelectList(await _context.TipoNota.Where(t => t.Eliminado == null).ToListAsync(), "Id", "Nombre", nota.TipoNotaId);
            ViewData["Usuarios"] = await _userManager.Users.ToListAsync();
            return View(nota);
        }

        // POST: Notas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Contenido,TipoNotaId,EsParaTodos,UsuarioId,Fecha")] Nota nota)
        {
            if (id != nota.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var entity = await _context.Nota.FindAsync(id);
                if (entity == null) return NotFound();

                entity.Titulo = nota.Titulo;
                entity.Contenido = nota.Contenido;
                entity.TipoNotaId = nota.TipoNotaId;
                entity.EsParaTodos = nota.EsParaTodos;
                entity.Fecha = nota.Fecha;
                entity.UsuarioId = nota.UsuarioId;
                entity.Editado = DateTime.UtcNow;
                entity.EditadoPor = _userManager.GetUserId(User);

                _context.Update(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoNotaId"] = new SelectList(await _context.TipoNota.Where(t => t.Eliminado == null).ToListAsync(), "Id", "Nombre", nota.TipoNotaId);
            ViewData["Usuarios"] = await _userManager.Users.ToListAsync();
            return View(nota);
        }

        // GET: Notas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var nota = await _context.Nota.Include(n => n.TipoNota).FirstOrDefaultAsync(m => m.Id == id && m.Eliminado == null);
            if (nota == null) return NotFound();

            return View(nota);
        }

        // POST: Notas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nota = await _context.Nota.FindAsync(id);
            if (nota == null) return NotFound();

            nota.Eliminado = DateTime.UtcNow;
            nota.EliminadoPor = _userManager.GetUserId(User);
            _context.Update(nota);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
