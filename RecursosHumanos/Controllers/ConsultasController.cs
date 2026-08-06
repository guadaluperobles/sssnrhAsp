using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Models;
using RecursosHumanos.Data;

public class ConsultasController : Controller
{
    private readonly ApplicationDbContext _context;
    public ConsultasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CONSULTASS
    public async Task<IActionResult> Index() {
        var consultas = await _context.Consulta.ToListAsync();
        var modelo = new CustomTable {
            Datos = consultas,
            Columnas = new List<CustomTableColumn> {
                new CustomTableColumn {
                    Propiedad = "Id",
                    Titulo = "Id",
                    Visible = false,
                    Pk = true
                }, new CustomTableColumn {
                    Propiedad = "Nombre",
                    Titulo = "Nombre"
                },
                new CustomTableColumn {
                    Propiedad = "Sql",
                    Titulo = "Consulta"
                },
            }
        };
        return View(modelo);
    }

    // GET: CONSULTASS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var consultas = await _context.Consulta
            .FirstOrDefaultAsync(m => m.Id == id);
        if (consultas == null)
        {
            return NotFound();
        }

        return View(consultas);
    }

    // GET: CONSULTASS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CONSULTASS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Sql,Activo,Creado,Editado,Eliminado")] Consultas consultas)
    {
        if (ModelState.IsValid)
        {
            _context.Add(consultas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(consultas);
    }

    // GET: CONSULTASS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var consultas = await _context.Consulta.FindAsync(id);
        if (consultas == null)
        {
            return NotFound();
        }
        return View(consultas);
    }

    // POST: CONSULTASS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nombre,Sql,Activo,Creado,Editado,Eliminado")] Consultas consultas)
    {
        if (id != consultas.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(consultas);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsultasExists(consultas.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(consultas);
    }

    // GET: CONSULTASS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var consultas = await _context.Consulta
            .FirstOrDefaultAsync(m => m.Id == id);
        if (consultas == null)
        {
            return NotFound();
        }

        return View(consultas);
    }

    // POST: CONSULTASS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var consultas = await _context.Consulta.FindAsync(id);
        if (consultas != null)
        {
            _context.Consulta.Remove(consultas);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ConsultasExists(int? id)
    {
        return _context.Consulta.Any(e => e.Id == id);
    }
}
