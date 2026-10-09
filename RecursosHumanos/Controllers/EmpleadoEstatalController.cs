using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Models;

[Authorize]
public class EmpleadoEstatalController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmpleadoEstatalController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var lista = await _context.EmpleadoEstatal.OrderBy(x => x.nombre).ToListAsync();
        return View(lista);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (id == null) return NotFound();
        var item = await _context.EmpleadoEstatal.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    public IActionResult Create()
    {
        return View(new EmpleadoEstatal { creado = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,nemp,pension,nombre,paterno,materno,curp,rfc,sexo")]
        EmpleadoEstatal empleado)
    {
        if (ModelState.IsValid)
        {
            empleado.creado = DateTime.Now;
            empleado.modificado = DateTime.Now;
            _context.Add(empleado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(empleado);
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (id == null) return NotFound();
        var empleado = await _context.EmpleadoEstatal.FindAsync(id);
        if (empleado == null) return NotFound();
        return View(empleado);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,nemp,pension,nombre,paterno,materno,curp,rfc,sexo,creado")]
        EmpleadoEstatal empleado)
    {
        if (id != empleado.Id) return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                empleado.modificado = DateTime.Now;
                _context.Update(empleado);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EmpleadoEstatal.Any(e => e.Id == empleado.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(empleado);
    }

    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();
        var empleado = await _context.EmpleadoEstatal.FirstOrDefaultAsync(x => x.Id == id);
        if (empleado == null) return NotFound();
        return View(empleado);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var empleado = await _context.EmpleadoEstatal.FindAsync(id);
        if (empleado != null)
        {
            _context.EmpleadoEstatal.Remove(empleado);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
