using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RecursosHumanos.Data;
using RecursosHumanos.Model;
using RecursosHumanos.Models;
using RecursosHumanos.ViewModel;
using System.Data;

namespace RecursosHumanos.Controllers {
    public class NominaController : Controller {
        // GET: NominaController1
        private readonly ConeccionService _coneccionService; 
        public NominaController(ConeccionService coneccionService) {
            _coneccionService = coneccionService;
        }
        public ActionResult Index() {

            var modelView = new NominaViewModel {
                BaseDatos = "SYSNGFSON_IB",
                Quincena = 15,
            };
            return View(modelView);
        }

        // GET: NominaController1/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: NominaController1/Create
        public ActionResult Create() {
            return View();
        }

        // POST: NominaController1/Create
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

        // GET: NominaController1/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: NominaController1/Edit/5
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

        public ActionResult PerDedEmpleado() {

            var modelView = new NominaViewModel {
                BaseDatos = "SYSNGFSON_IB",
                Quincena = 15,
            };
            return View(modelView);
        }

        // GET: NominaController1/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: NominaController1/Delete/5
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
        public async Task<IActionResult> Importar(IFormFile archivo, string BaseDatos, int Quincena) {
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

            DataRow filaTitulos = dt.Rows[0];

            for (int i = 0; i < dt.Columns.Count; i++) {
                string titulo = filaTitulos[i]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(titulo))
                    titulo = $"Column{i}";

                string nombreOriginal = titulo;
                int contador = 0;

                while (dt.Columns.Cast<DataColumn>()
                    .Any(c => c.ColumnName == titulo && c.Ordinal != i)) {
                    titulo = $"{nombreOriginal}_{contador++}";
                }

                dt.Columns[i].ColumnName = titulo;
            }

            dt.Rows.RemoveAt(0);

            DataTable dtPerDed_Empleado = new DataTable();
            DataTable dtPerDed_Empleado_404 = dt.Clone();

            dtPerDed_Empleado.Columns.Add("ClkDet", typeof(Int32));
            dtPerDed_Empleado.Columns.Add("MePDTipo", typeof(Int16));
            dtPerDed_Empleado.Columns.Add("MePDClave", typeof(string));
            dtPerDed_Empleado.Columns.Add("MePDVParA", typeof(string));
            dtPerDed_Empleado.Columns.Add("MePDVImp", typeof(decimal));
            dtPerDed_Empleado.Columns.Add("MePDVVigI", typeof(string));
            dtPerDed_Empleado.Columns.Add("MePDVVenc", typeof(string));

            Global global = new Global(_coneccionService);

            foreach (DataRow row in dt.Rows) {
                int numEmp = Convert.ToInt32(row["No.Emp"].ToString());
                string Filiacion = row["Filiacion"].ToString();

                string ConsultaClkDetRfc = $"SELECT ClkDet FROM Empleado WHERE ClkDet = {numEmp} AND MeRfc = '{Filiacion}'";
                string ConsultaRfc = $"SELECT ClkDet FROM Empleado WHERE MeRfc = '{Filiacion}'";

                DataTable numeroEmpleadoValidado = global.ConsultaGeneral(ConsultaClkDetRfc, BaseDatos);
                DataTable numeroEmpleado;

                if (numeroEmpleadoValidado == null || numeroEmpleadoValidado.Rows.Count == 0)
                    numeroEmpleado = global.ConsultaGeneral(ConsultaRfc, BaseDatos);
                else
                    numeroEmpleado = numeroEmpleadoValidado;

                int clkDet = 0;

                if (numeroEmpleado != null && numeroEmpleado.Rows.Count > 0) {
                    clkDet = Convert.ToInt32(numeroEmpleado.Rows[0]["ClkDet"]);
                }

                if (clkDet > 0)
                    foreach (DataColumn columna in dt.Columns) {
                        // Saltar la columna numEmp
                        if (columna.ColumnName == "ClkDet")
                            continue;

                        var valor = row[columna];

                        // Ignorar valores vacíos
                        if (valor == DBNull.Value ||
                            string.IsNullOrWhiteSpace(valor.ToString()))
                            continue;

                        DataRow nuevaFila = dtPerDed_Empleado.NewRow();
                        string TipoClave = columna.ColumnName;

                        string tipo = TipoClave.Substring(0, 1);   // "2"

                        if (tipo != "1" && tipo != "2")
                            continue;

                        string clave = TipoClave.Substring(1);

                        nuevaFila["ClkDet"] = numEmp;
                        nuevaFila["MePDTipo"] = tipo;
                        nuevaFila["MePDClave"] = clave;
                        nuevaFila["MePDVParA"] = "00";
                        nuevaFila["MePDVImp"] = Convert.ToDecimal(valor);
                        nuevaFila["MePDVVigI"] = "000000";
                        nuevaFila["MePDVVenc"] = "000000";


                        if (clkDet != 0)
                            dtPerDed_Empleado.Rows.Add(nuevaFila);
                    }
                else
                    dtPerDed_Empleado_404.ImportRow(row);
            }

            if (dtPerDed_Empleado.Rows.Count > 0) {
                
                global.ConsultaGeneral("DELETE FROM PerDed_Empleado", BaseDatos);
                
                foreach (DataRow row in dtPerDed_Empleado.Rows) {
                    Decimal Monto = Convert.ToDecimal(row["MePDVImp"]);
                    if (Monto <= 0)
                        continue;
                    string sql = $@"
                        INSERT INTO PerDed_Empleado
                        (
                            ClkDet,
                            MePDTipo,
                            MePDClave,
                            MePDVParA,
                            MePDVImp,
                            MePDVVigI,
                            MePDVVenc
                        )
                        VALUES
                        (
                            {Convert.ToInt32(row["ClkDet"])},
                            '{row["MePDTipo"]}',
                            '{row["MePDClave"]}',
                            '{row["MePDVParA"]}',
                            {Monto},
                            '{row["MePDVVigI"]}',
                            '{row["MePDVVenc"]}'
                        )";
                    global.ConsultaGeneral(sql, BaseDatos);
                }
            }

            var modelView = new NominaViewModel {
                BaseDatos = BaseDatos,
                Quincena = Quincena,
                Procesados = dtPerDed_Empleado,
                EmpleadosFaltantes = dtPerDed_Empleado_404,
            };
            // Ya puedes trabajar con el DataTable
            return View("Index", modelView);
        }

    }

}
