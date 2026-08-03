using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class ChequeController : Controller {
    private readonly ApplicationDbContext _context;

    public ChequeController(ApplicationDbContext context) {
        _context = context;
    }

    // GET: CHEQUES
    public async Task<IActionResult> Index() {
        var cheques = await _context.Cheque.ToListAsync();
        var modelo = new CustomTable {
            Datos = cheques,
            Columnas = new List<CustomTableColumn> {
                new CustomTableColumn {
                    Propiedad = "Id",
                    Titulo = "Id",
                    Visible = false,
                    Pk = true
                }, new CustomTableColumn {
                    Propiedad = "Ejercicio",
                    Titulo = "Ejercicio"
                },
                new CustomTableColumn {
                    Propiedad = "Quincena",
                    Titulo = "Quincena"
                },
                new CustomTableColumn {
                    Propiedad = "NumeroCheque" ,
                    Titulo = "Num de Cheque"
                },
                new CustomTableColumn {
                    Propiedad = "FechaPago" ,
                    Titulo = "Fecha de pago"
                },
                new CustomTableColumn {
                    Propiedad = "ClkDet" ,
                    Titulo = "Número de Empleado"
                },
                new CustomTableColumn {
                    Propiedad = "NombreEmpleado" ,
                    Titulo = "Empleado"
                },
                new CustomTableColumn {
                    Propiedad = "RfcEmpleado" ,
                    Titulo = "RFC"
                },
                new CustomTableColumn {
                    Propiedad = "NombreBeneficiario" ,
                    Titulo = "Beneficiario"
                }
            }
        };
        return View(modelo);
    }

    // GET: CHEQUES/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (id == null) {
            return NotFound();
        }

        var cheque = await _context.Cheque
            .FirstOrDefaultAsync(m => m.Id == id);
        if (cheque == null) {
            return NotFound();
        }

        return View(cheque);
    }

    // GET: CHEQUES/Create
    public IActionResult Create() {
        return View();
    }

    // POST: CHEQUES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ClkDet,NombreEmpleado,NombreBeneficiario,RfcEmpleado,ClavePresupuestal,NumeroCheque,ClaveUbicacion,Descripcion,InicioPeriodo,FinPeriodo,FechaPago,Neto,Deducciones,VPA,VPATexto,impreso,Ejercicio,Quincena")] Cheque cheque) {
        if (ModelState.IsValid) {
            _context.Add(cheque);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(cheque);
    }

    // GET: CHEQUES/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (id == null) {
            return NotFound();
        }

        var cheque = await _context.Cheque.FindAsync(id);
        if (cheque == null) {
            return NotFound();
        }
        return View(cheque);
    }

    // POST: CHEQUES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,ClkDet,NombreEmpleado,NombreBeneficiario,RfcEmpleado,ClavePresupuestal,NumeroCheque,ClaveUbicacion,Descripcion,InicioPeriodo,FinPeriodo,FechaPago,Neto,Deducciones,VPA,VPATexto,impreso,Ejercicio,Quincena")] Cheque cheque) {
        if (id != cheque.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                _context.Update(cheque);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ChequeExists(cheque.Id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(cheque);
    }

    // GET: CHEQUES/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (id == null) {
            return NotFound();
        }

        var cheque = await _context.Cheque
            .FirstOrDefaultAsync(m => m.Id == id);
        if (cheque == null) {
            return NotFound();
        }

        return View(cheque);
    }

    // POST: CHEQUES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id) {
        var cheque = await _context.Cheque.FindAsync(id);
        if (cheque != null) {
            _context.Cheque.Remove(cheque);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ChequeExists(int? id) {
        return _context.Cheque.Any(e => e.Id == id);
    }
    public async Task<IActionResult> GenerarReporte(int id) {
        try {
            var ruta = Path.Combine( Directory.GetCurrentDirectory(), "Reportes","rptImpresionCheque.rdlc");

            if (!System.IO.File.Exists(ruta)) {
                return NotFound("No se encontró el archivo RDLC.");
            }

            var cheque = await ObtenerDatos(id);

            if (cheque == null) {
                return NotFound("No se encontró el cheque.");
            }

            var reporte = new LocalReport { ReportPath = ruta };
            reporte.DataSources.Add(new ReportDataSource("dsCheque",new List<Cheque> { cheque }) );

            byte[] pdf = reporte.Render("PDF");

            return File(pdf, "application/pdf");
        }
        catch (Exception ex) {
            return Content(ex.ToString());
        }
    }

    public async Task<Cheque> ObtenerDatos(int id) {
        return await _context.Cheque.FindAsync(id);
    }
}
