using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.Differencing;
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
        //var cheques = await _context.Cheque.OrderBy(x => x.NumeroCheque).ToListAsync();


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

        DateTime fecha = DateTime.Now;
        string[] df = DatosFecha(fecha);

        int ejercicio = DateTime.Now.Year;
        int quincena = Convert.ToInt32(df[0]);
        var ultimoCheque = await _context.Cheque.Where(x => x.TipoCheque == "PensionAlimenticia").OrderByDescending(x => x.NumeroCheque).Select(x => x.NumeroCheque).FirstOrDefaultAsync();
        var digito = (ultimoCheque ?? 0) + 1;
        
        
        var modelView = new ChequeViewModel {
            Cheques = modelo,
            Quincena = quincena,
            Ejercicio = ejercicio,
            UltimoDigito = digito
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
    public async Task<IActionResult> Create() {
        DateTime fecha = DateTime.Now;
        string[] df = DatosFecha(fecha);

        int ejercicio = DateTime.Now.Year;
        int quincena = Convert.ToInt32(df[0]);
        var ultimoCheque = await _context.Cheque.Where(x => x.TipoCheque == "PensionAlimenticia").OrderByDescending(x => x.NumeroCheque).FirstOrDefaultAsync();

        var model = new Cheque {
            Quincena = quincena,
            Ejercicio = ejercicio,
            NumeroCheque = ultimoCheque.NumeroCheque.Value + 1,
            TipoCheque = "PensionAlimenticia",
            FechaPago = DateTime.Now
        };

        return View(model);

    }

    // POST: CHEQUES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ClkDet,NombreEmpleado,NombreBeneficiario,RfcEmpleado,ClavePresupuestal,NumeroCheque,ClaveUbicacion,Descripcion,InicioPeriodo,FinPeriodo,FechaPago,Neto,Deducciones,VPA,VPATexto,impreso,Ejercicio,Quincena,TipoCheque")] Cheque cheque) {
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
    public async Task<IActionResult> Edit(int? id, [Bind("Id,ClkDet,NombreEmpleado,NombreBeneficiario,RfcEmpleado,ClavePresupuestal,NumeroCheque,ClaveUbicacion,Descripcion,InicioPeriodo,FinPeriodo,FechaPago,Neto,Deducciones,VPA,VPATexto,impreso,Ejercicio,Quincena,TipoCheque")] Cheque cheque) {

        cheque.VPATexto = Global.Letras((cheque.VPA).ToString());
        cheque.impreso = 0;

        ModelState.SetModelValue("VPATexto", new ValueProviderResult(cheque.VPATexto));

        ModelState.Remove("VPATexto");
        ModelState.Remove("TipoCheque");

        if (id == null) {
            cheque.Creado = DateTime.Now;

            if (ModelState.IsValid) {
                _context.Cheque.Add(cheque);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(cheque);
        }

        if (id != cheque.Id) {
            return NotFound();
        }

        cheque.Editado = DateTime.Now;

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
    public async Task<IActionResult> Importar(IFormFile archivo, int Ejercicio, int Quincena, int NumeroCheque) {
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

        DataRow filaTitulos = dt.Rows[1];

        for (int i = 0; i < dt.Columns.Count; i++) {
            string titulo = filaTitulos[i]?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(titulo))
                titulo = $"Column{i}";

            string nombreOriginal = titulo;
            int contador = 1;

            while (dt.Columns.Cast<DataColumn>()
                .Any(c => c.ColumnName == titulo && c.Ordinal != i)) {
                titulo = $"{nombreOriginal}_{contador++}";
            }

            dt.Columns[i].ColumnName = titulo;
        }

        dt.Rows.RemoveAt(1);
        dt.Rows.RemoveAt(0);

        // Ya puedes trabajar con el DataTable
        return await GuardarExcel(dt, Ejercicio, Quincena, NumeroCheque);
    }
    private async Task<IActionResult> GuardarExcel(DataTable re, int Ejercicio,int Quincena,int NumeroCheque) {
        try {
            DataTable filtrado = re.AsEnumerable().Where(r => r.Field<string>(4)?.Contains("CHEQUE", StringComparison.OrdinalIgnoreCase) == true).CopyToDataTable();

            foreach (DataRow row in filtrado.Rows) {
                var identificador = row["Descripcion"]?.ToString()?.Trim().ToUpper();
                if (identificador == "CHEQUE") {
                    var nombreBeneficiario = $"{row["Nombre"].ToString()} {row["apPaterno"].ToString()} {row["apPaterno"].ToString()} ";
                    var nombreEmpleado = $"{row["Nombre_1"].ToString()} {row["apPaterno_1"].ToString()} {row["apMaterno_1"].ToString()}";
                    var numeroEmpleado = Convert.ToInt32(row["NumEmp_1"].ToString());

                    var ultimoChequeEmpleado = await _context.Cheque.Where(x => x.ClkDet == numeroEmpleado).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    var ultimoCheque = await _context.Cheque.Where(x => x.TipoCheque == "PensionAlimenticia").OrderByDescending(x => x.NumeroCheque).FirstOrDefaultAsync();

                    var digito = (ultimoCheque?.NumeroCheque ?? 0) + 1;
                    DateTime fecha = DateTime.Now;
                    string[] df = DatosFecha(fecha);

                    var cheque = new Cheque {
                        ClkDet = Convert.ToInt32(numeroEmpleado),
                        NombreEmpleado = nombreEmpleado,
                        NombreBeneficiario = nombreBeneficiario,
                        RfcEmpleado = ultimoChequeEmpleado.RfcEmpleado,
                        ClavePresupuestal = ultimoChequeEmpleado.ClavePresupuestal,
                        NumeroCheque = digito, //"ultimo cheque global"
                        ClaveUbicacion = ultimoChequeEmpleado.ClaveUbicacion,
                        Descripcion = "PA",
                        InicioPeriodo = Global.ObtenerFecha(df[3].ToString()),
                        FinPeriodo = Global.ObtenerFecha(df[4].ToString()),
                        FechaPago = Global.ObtenerFecha(df[2].ToString()),
                        Neto = Global.ObtenerDecimal(row["Valor"].ToString()),
                        Deducciones = Global.ObtenerDecimal("0.00"),
                        VPA = Global.ObtenerDecimal(row["Valor"].ToString()),
                        VPATexto = Global.Letras(row["Valor"].ToString()),
                        impreso = 0,
                        Creado = fecha,
                        Ejercicio = Convert.ToInt32(df[1]),
                        Quincena = Convert.ToInt32(df[0]),
                        TipoCheque = "PensionAlimenticia"
                    };

                    await GuardarCheque(cheque);
                }
            }
            return RedirectToAction("Index");
        }
        catch(Exception ex) {
            return BadRequest($"error. {ex.Message}");
        }
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
    [HttpPost]
    public async Task<IActionResult> ObtenerDatos(string tipoCheque) {
        var cheque = await _context.Cheque
            .Where(x => x.TipoCheque == tipoCheque)
            .OrderByDescending(x => x.NumeroCheque)
            .FirstOrDefaultAsync();

        if (cheque == null)
            return NotFound();

        return Json(new {
            numeroCheque = cheque.NumeroCheque,
            nombreEmpleado = cheque.NombreEmpleado,
            neto = cheque.Neto,
            vpa = cheque.VPA
        });
    }
    public async Task<IActionResult> GenerarReporteCheque(int id, string tipo) {
        try {
            string ruta;
            var cheque = await _context.Cheque.FirstOrDefaultAsync(x => x.Id == id);

            if (cheque.TipoCheque == "Chequera")
                ruta = Path.Combine(Directory.GetCurrentDirectory(), "Reportes", "rptImpresionChequera.rdlc");
            else
                ruta = Path.Combine(Directory.GetCurrentDirectory(), "Reportes", "rptImpresionCheque.rdlc");

            if (!System.IO.File.Exists(ruta)) 
                return NotFound("No se encontró el archivo RDLC.");
            

            if (cheque == null) 
                return NotFound("No se encontró el cheque.");
            

            var reporte = new LocalReport {
                ReportPath = ruta
            };

            reporte.DataSources.Add(
                new ReportDataSource(
                    "dsCheque",
                    new List<Cheque> { cheque }
                )
            );

            // Marcar como impreso
            cheque.impreso = 1;
            cheque.Impresion = DateTime.Now;
            await _context.SaveChangesAsync();
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
    private string[] DatosFecha(DateTime fa, int quincena = 0) {
        if(quincena == 0)
            quincena = (fa.Month - 1) * 2 + (fa.Day <= 15 ? 1 : 2);

        Periodo periodo = new Periodo().quincena().FirstOrDefault(p => p.Quincenas == quincena.ToString());
        DateTime PrimerDiaQuincena, ultimoDiaQuincena;

        if (periodo.Inicio == "1") {
            PrimerDiaQuincena = new DateTime(fa.Year, fa.Month, 1);
            ultimoDiaQuincena = new DateTime(fa.Year, fa.Month, 15);
        }
        else {
            PrimerDiaQuincena = new DateTime(fa.Year, fa.Month, 16);
            ultimoDiaQuincena = new DateTime(fa.Year, fa.Month, DateTime.DaysInMonth(fa.Year, fa.Month));
        }

        string fechaPago = ultimoDiaQuincena.ToString("dd/MM/yyyy");
        string fechaInicioPeriodo = PrimerDiaQuincena.ToString("dd/MM/yyyy");
        string fechaFinPeriodo = ultimoDiaQuincena.ToString("dd/MM/yyyy");

        return new string[]
        {
                quincena.ToString("00"),
                fa.Year.ToString(),
                fechaPago,
                fechaInicioPeriodo,
                fechaFinPeriodo
        };
    }
}
