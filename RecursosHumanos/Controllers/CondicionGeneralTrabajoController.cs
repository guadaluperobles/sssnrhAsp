using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.ViewModel;
using System.Data;

namespace RecursosHumanos.Controllers {
    public class CondicionGeneralTrabajoController : Controller {
        // GET: CondicionGeneralTrabajoController

        private readonly ApplicationDbContext _context;
        private readonly Global _global;
        public CondicionGeneralTrabajoController(ApplicationDbContext context, Global global) {
            _context = context;
            _global = global;
        }
        // GET: CentroTrabajoController
        public async Task<IActionResult> Index() {
            int Ejercicio = DateTime.Now.Year;
            int mes = DateTime.Now.Month;
            int Semestre = ((mes - 1) / 4) + 1;

            if (mes >= 1 && mes <= 6)
                Semestre = 1;
            else {
                Semestre = 2;
            }

            var Resultado = ProcesarConsulta(Ejercicio, Semestre);

            var Model = new ConsultaSemestralViewModel {
                Ejercicio = Ejercicio,
                Semestre = Semestre,
                Models = await Resultado
            };
            return View(Model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int Ejercicio, int Semestre) {
            var Resultado = ProcesarConsulta(Ejercicio, Semestre);

            var Model = new ConsultaSemestralViewModel {
                Ejercicio = Ejercicio,
                Semestre = Semestre,
                Models = await Resultado
            };
            return View(Model);
        }

        // GET: CondicionGeneralTrabajoController/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: CondicionGeneralTrabajoController/Create
        public ActionResult Create() {
            return View();
        }

        // POST: CondicionGeneralTrabajoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

        // GET: CondicionGeneralTrabajoController/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: CondicionGeneralTrabajoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }

        // GET: CondicionGeneralTrabajoController/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: CondicionGeneralTrabajoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                return RedirectToAction(nameof(Index));
            }
            catch {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarExcel(ConsultaSemestralViewModel model) {
            try {
                var Resultado = await ProcesarConsulta(model.Ejercicio, model.Semestre);
                var tablas = new Dictionary<string, DataTable>{
                    { "Hoja 1", Resultado }
                };
                return ArchivoController.ExportarExcel(tablas, $"CondicionGeneralTrabajo {model.Ejercicio}/{model.Semestre}");
            }
            catch (Exception ex) {
                return BadRequest(ex.Message + "56");
            }
        }
        public async Task<DataTable> ProcesarConsulta(int ejercicio, int semestre) {

            var consultaBD = await _context.Consulta.FirstOrDefaultAsync(x => x.Nombre == "CondicionGeneralTrabajo");
            int ejercicioAnterior = ejercicio;
            int QnaInicio = 0, QnaFin = 0;
            if (semestre == 1) {
                QnaInicio = 1; 
                QnaFin = 12;
            }
            else {
                QnaInicio = 13; 
                QnaFin = 24;
            }

            if (consultaBD == null)
                throw new Exception("No se encontró la consulta CondicionGeneralTrabajo.");

            var consultaLocalTrimActual = $@" DECLARE @Anio			            INT = {ejercicio}; 
                                              DECLARE @quincenaInicio		    INT = {QnaInicio};                             
                                              DECLARE @QuincenaFin		        INT = {QnaFin};";
            var resultado = new DataTable();

            resultado.Columns.Add("N0.ENTIDAD FEDERATIVA");
            resultado.Columns.Add("ENTIDAD FEDERATIVA");
            resultado.Columns.Add("CONCEPTO DE PAGO");
            resultado.Columns.Add("DESCRIPCIÓN DE CONCEPTO DE PAGO");
            resultado.Columns.Add("TIPO DE CONTRATACIÓN");
            resultado.Columns.Add("NOMBRE DEL TRABAJADOR");
            resultado.Columns.Add("RFC");
            resultado.Columns.Add("TIPO DE NÓMINA");
            resultado.Columns.Add("CÓDIGO DE PUESTO");
            resultado.Columns.Add("FECHA DE PAGO");
            resultado.Columns.Add("IMPORTE BRUTO");
            resultado.Columns.Add("ISR");
            resultado.Columns.Add("IMPORTE NETO");

            var resultadoTrimActual = _global.ConsultaGeneral(consultaLocalTrimActual + consultaBD.Sql);
            var bdOperativas = _global.BasesDatosOperativas();

            if (resultadoTrimActual != null || resultadoTrimActual.Rows.Count > 0)
                foreach (DataRow Row in resultadoTrimActual.Rows) {
                    var fecha = Row[6]?.ToString()?.Trim();
                    string tipoNomina = "";

                    string fechaFormateada = "";

                    if (DateTime.TryParseExact(
                        fecha,
                        "yyyyMMdd",
                        null,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime fechaResult)) {
                        fechaFormateada = fechaResult.ToString("dd/MM/yyyy");
                    }

                    Decimal Bruto = Global.ObtenerDecimal(Row[7].ToString());
                    Decimal Iva = Global.ObtenerDecimal(Row[8].ToString());
                    Decimal Neto = string.IsNullOrWhiteSpace(Row[9]?.ToString()) ? Bruto - Iva : Global.ObtenerDecimal(Row[9].ToString());

                    Neto = Global.ObtenerDecimal(Row[8].ToString());

                    DataRow bd = bdOperativas.Select($"BaseDatos = '{Row[11].ToString()}'").FirstOrDefault();
                    string descripcionBaseDatos = bd?["Descripcion"]?.ToString() ?? "";

                    switch (Row[5].ToString()) {
                        case "PRE":
                            tipoNomina = "EXTRAORDINARIA";
                            break;
                        case "PRO":
                            tipoNomina = "ORDINARIA";
                            break;
                        case "PRR":
                            tipoNomina = "RETROACTIVA";
                            break;
                        case "PRC":
                            tipoNomina = "RECALCULO";
                            break;
                        case "PRX":
                            tipoNomina = "REEXPEDICIÓN";
                            break;
                        case "PRA":
                            tipoNomina = "AGUINALDO";
                            break;
                    }

                            resultado.Rows.Add(
                        "26",
                        "SONORA",
                        Row[0],
                        Row[1],
                        descripcionBaseDatos,
                        Row[2],
                        Row[3],
                        tipoNomina,
                        Row[4],
                        fechaFormateada,
                        Bruto,
                        Iva,
                        Neto
                        );
                }

            else
                return resultado;

            try {

                return resultado;
            }
            catch (Exception ex) {
                return resultado;
            }
        }    
    }
}
