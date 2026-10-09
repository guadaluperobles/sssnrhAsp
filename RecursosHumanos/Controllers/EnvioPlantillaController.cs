using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;

[Authorize]
public class EnvioPlantillaController : Controller
{
    private readonly ApplicationDbContext _context;

    public EnvioPlantillaController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var lista = await _context.EnvioPlantilla.OrderByDescending(x => x.creado).ToListAsync();
        return View(lista);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (id == null) return NotFound();
        var item = await _context.EnvioPlantilla.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    public IActionResult Create()
    {
        return View(new EnvioPlantilla { creado = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Correo,Plantilla,Titulo,Responsable,descripcion")] EnvioPlantilla envio)
    {
        if (ModelState.IsValid)
        {
            envio.creado = DateTime.Now;
            envio.modificado = DateTime.Now;
            _context.Add(envio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(envio);
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (id == null) return NotFound();
        var envio = await _context.EnvioPlantilla.FindAsync(id);
        if (envio == null) return NotFound();
        return View(envio);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Correo,Plantilla,Titulo,Responsable,descripcion,creado")] EnvioPlantilla envio)
    {
        if (id != envio.Id) return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                envio.modificado = DateTime.Now;
                _context.Update(envio);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EnvioPlantilla.Any(e => e.Id == envio.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(envio);
    }

    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();
        var envio = await _context.EnvioPlantilla.FirstOrDefaultAsync(x => x.Id == id);
        if (envio == null) return NotFound();
        return View(envio);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var envio = await _context.EnvioPlantilla.FindAsync(id);
        if (envio != null)
        {
            _context.EnvioPlantilla.Remove(envio);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
