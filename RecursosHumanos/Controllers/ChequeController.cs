using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class ChequeController : Controller {
    private readonly ApplicationDbContext _context;

    public ChequeController(ApplicationDbContext context) {
        _context = context;
    }

    // GET: CHEQUES
    public async Task<IActionResult> Index() {
        var cheques = await _context.Cheque.OrderByDescending(x => x.NumeroCheque).ToListAsync();

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
            },
        };
        int ejercicio = 2026;
        int quincena = 1;

        var modelView = new ChequeViewModel {
            Cheques = modelo,
            Quincena = quincena,
            Ejercicio = ejercicio
        };

        return View(modelView);
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
    public async Task<IActionResult> Exportar(IFormFile archivo) {
        if (archivo == null)
            return BadRequest();

        DataTable dt;

        switch (Path.GetExtension(archivo.FileName).ToLower()) {
            case ".xlsx":
            case ".xls":
                dt = RecursosHumanos.Controllers.ArchivoController.DtLeerExcel(archivo);
                break;

            default:
                return BadRequest("Formato no soportado.");
        }

        // Ya puedes trabajar con el DataTable
        return await GuardarExcel(dt);
    }
    private async Task<IActionResult> GuardarExcel(DataTable re) {
        DataTable filtrado = re.AsEnumerable().Where(r => r.Field<string>(4)?.Contains("CHEQUE", StringComparison.OrdinalIgnoreCase) == true).CopyToDataTable();

        foreach (DataRow row in filtrado.Rows) {
            var identificador = row[4]?.ToString()?.Trim().ToUpper();
            if (identificador == "CHEQUE") {
                var nombreBeneficiario = $"{row[3].ToString()} {row[1].ToString()} {row[2].ToString()} ";
                var nombreEmpleado = $"{row[12].ToString()} {row[10].ToString()} {row[11].ToString()}";
                var numeroEmpleado = Convert.ToInt32(row[9].ToString());
                
                var ultimoChequeEmpleado = await _context.Cheque.Where(x => x.ClkDet == numeroEmpleado).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                var ultimoCheque = await _context.Cheque.OrderByDescending(x => x.NumeroCheque).FirstOrDefaultAsync();

                var cheque = new Cheque {
                    ClkDet = Convert.ToInt32(numeroEmpleado),
                    NombreEmpleado = nombreEmpleado,
                    NombreBeneficiario = nombreBeneficiario,
                    RfcEmpleado = ultimoChequeEmpleado.RfcEmpleado,
                    ClavePresupuestal = ultimoChequeEmpleado.ClavePresupuestal,
                    NumeroCheque = ultimoCheque.NumeroCheque + 1, //"ultimo cheque global"
                    ClaveUbicacion = ultimoChequeEmpleado.ClaveUbicacion,
                    Descripcion = "PA",
                    //InicioPeriodo = Global.ObtenerFecha(df[3].ToString()),
                    //FinPeriodo = Global.ObtenerFecha(df[4].ToString()),
                    //FechaPago = Global.ObtenerFecha(df[2].ToString()),
                    Neto = Global.ObtenerDecimal(row[7].ToString()),
                    Deducciones = Global.ObtenerDecimal("0.00"),
                    VPA = Global.ObtenerDecimal(row[7].ToString()),
                    VPATexto = Global.Letras(row[7].ToString()),
                    impreso = 0
                    //Ejercicio = Convert.ToInt32(df[1]),
                    //Quincena = Convert.ToInt32(df[0]),
                };

                if (await GuardarCheque(cheque)) {

                }
                else { 
                
                }
            }
        }

        return await Index();
    }
    private async Task<bool> GuardarCheque(Cheque cheque) {
        try {
            _context.Cheque.Add(cheque);
            await _context.SaveChangesAsync();
            return true;
        }
        catch {
            return false;
        }
    }
    private bool ChequeExists(int? id) {
        return _context.Cheque.Any(e => e.Id == id);
    }
    public async Task<IActionResult> GenerarReporteCheque(int id) {
        try {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "Reportes", "rptImpresionCheque.rdlc");

            if (!System.IO.File.Exists(ruta)) {
                return NotFound("No se encontró el archivo RDLC.");
            }

            var cheque = await ObtenerDatos(id);

            if (cheque == null) {
                return NotFound("No se encontró el cheque.");
            }

            var reporte = new LocalReport { ReportPath = ruta };
            reporte.DataSources.Add(new ReportDataSource("dsCheque", new List<Cheque> { cheque }));

            byte[] pdf = reporte.Render("PDF");

            return File(pdf, "application/pdf");
        }
        catch (Exception ex) {
            return Content(ex.ToString());
        }
    }
    public async Task<IActionResult> GenerarReporteFirmas(int id) {
        try {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "Reportes", "rptFirmaCheque.rdlc");

            if (!System.IO.File.Exists(ruta)) {
                return NotFound("No se encontró el archivo RDLC.");
            }

            var cheque = await ObtenerDatos(id);

            if (cheque == null) {
                return NotFound("No se encontró el cheque.");
            }

            var reporte = new LocalReport { ReportPath = ruta };
            reporte.DataSources.Add(new ReportDataSource("dsCheque", new List<Cheque> { cheque }));

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
