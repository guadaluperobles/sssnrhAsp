using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Models;
using RecursosHumanos.Data;

public class PermisoVistaController : Controller
{
    private readonly ApplicationDbContext _context;
    public PermisoVistaController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PermisoVistaModels
    public async Task<IActionResult> Index()
    {
        var permisos = await _context.PermisoVistaModel.ToListAsync();
        var modelo = new CustomTable {
            Datos = permisos,
            Columnas = new List<CustomTableColumn> {
                new CustomTableColumn { Propiedad = "Id", Titulo = "Id", Visible = false, Pk = true },
                new CustomTableColumn { Propiedad = "Nombre", Titulo = "Nombre" },
                new CustomTableColumn { Propiedad = "Controller", Titulo = "Controller" },
                new CustomTableColumn { Propiedad = "Action", Titulo = "Action" },
            },
            Acciones = new List<CustomTableAction>{
                new CustomTableAction{ Titulo = "Editar", Action = "Edit", Controller = "PermisoVista", Clase = "btn btn-info", Icono = "bi bi-pencil-fill" },
                new CustomTableAction{ Titulo = "Eliminar", Action = "Delete", Controller = "PermisoVista", Clase = "btn btn-delete", Icono = "bi bi-trash" },
            }
        };
        return View(modelo);
    }

    // GET: PermisoVistaModels/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var permiso = await _context.PermisoVistaModel.FirstOrDefaultAsync(m => m.Id == id);
        if (permiso == null) return NotFound();

        return View(permiso);
    }

    // GET: PermisoVistaModels/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PermisoVistaModels/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Controller,Action,Descripcion,Activo,Creado,Editado,Eliminado")] PermisoVistaModel permisoVistaModel)
    {
        if (ModelState.IsValid)
        {
            _context.Add(permisoVistaModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(permisoVistaModel);
    }

    // GET: PermisoVistaModels/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var permiso = await _context.PermisoVistaModel.FindAsync(id);
        if (permiso == null) return NotFound();
        return View("Create", permiso);
    }

    // POST: PermisoVistaModels/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Controller,Action,Descripcion,Activo,Creado,Editado,Eliminado")] PermisoVistaModel permisoVistaModel)
    {
        if (id != permisoVistaModel.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(permisoVistaModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermisoVistaModelExists(permisoVistaModel.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(permisoVistaModel);
    }

    // GET: PermisoVistaModels/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var permiso = await _context.PermisoVistaModel.FirstOrDefaultAsync(m => m.Id == id);
        if (permiso == null) return NotFound();

        return View(permiso);
    }

    // POST: PermisoVistaModels/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var permiso = await _context.PermisoVistaModel.FindAsync(id);
        if (permiso != null)
        {
            _context.PermisoVistaModel.Remove(permiso);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PermisoVistaModelExists(int? id)
    {
        return _context.PermisoVistaModel.Any(e => e.Id == id);
    }
}
